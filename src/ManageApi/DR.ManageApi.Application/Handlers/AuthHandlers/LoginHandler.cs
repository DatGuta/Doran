using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DR.Common;
using DR.Redis;
using DR.Common.Exceptions;
using DR.Resource;
using DR.Common.Hashers;
using DR.Models;
using DR.Common.Helpers;
using DR.Constant.Enums;

namespace DR.Handlers.AuthHandlers {

    public class LoginReq : IRequest<LoginRes> {
        public string MerchantCode { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public EPermission Permission { get; set; }
    }

    public class LoginRes {
        public string RefreshToken { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string MerchantCode { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long ExpiredTime { get; set; }
        public long Session { get; set; }
    }

    public class LoginHandler : BaseHandler<LoginReq, LoginRes> {
        private readonly IRedisCacheService redisCacheService;

        public LoginHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.redisCacheService = serviceProvider.GetRequiredService<IRedisCacheService>();
        }

        public override async Task<LoginRes> Handle(LoginReq request, CancellationToken cancellationToken) {
            request.Username = request.Username.ToLower().Trim();
            return await this.Login(request, request.Permission, cancellationToken);
        }

        protected async Task<LoginRes> Login(LoginReq request, EPermission type, CancellationToken cancellationToken) {
            var merchant = await this.db.Merchants.AsNoTracking().FirstOrDefaultAsync(o => o.Code == request.MerchantCode, cancellationToken);
            ManagedException.ThrowIfNull(merchant, Messages.Auth.Login.Merchant_NotFound);
            ManagedException.ThrowIfFalse(merchant.IsActive, Messages.Auth.Login.Merchant_Inactive);
            ManagedException.ThrowIf(merchant.ExpiredDate <= DateTimeOffset.UtcNow, Messages.Auth.Login.Merchant_Expired);

            var user = await this.db.Users.FirstOrDefaultAsync(o => o.MerchantId == merchant.Id && o.Username == request.Username, cancellationToken);
            ManagedException.ThrowIfNull(user, Messages.Auth.Login.User_NotFound);
            ManagedException.ThrowIf(!user.IsAdmin && !user.IsActive, Messages.Auth.Login.User_Inactive);
            ManagedException.ThrowIf(!PasswordHasher.Verify(request.Password, user.Password), Messages.Auth.Login.User_IncorrectPassword);

            var permissions = await this.db.GetPermissions(o => o.Type == type, cancellationToken);
            Database.Models.Role? role = null;
            if (!string.IsNullOrWhiteSpace(user.RoleId)) {
                role = await this.db.Roles.Include(o => o.RolePermissions).AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == user.RoleId && o.MerchantId == merchant.Id);
            }

            var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
            ManagedException.ThrowIfFalse(userPermissions.Exists(o => o.IsEnable), Messages.Auth.Login.User_NoPermission);

            //Chuyển Guid sang String
            var claims = new List<Claim>() {
                new Claim(Constants.TokenMerchantId, merchant.Id.ToString()),
                new Claim(Constants.TokenUserId, user.Id),
            };

            if (user.IsAdmin) claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var session = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (type == EPermission.Web) {
                string source = type.ToString();

                claims.Add(new Claim(Constants.TokenSource, source));
                claims.Add(new Claim(Constants.TokenSession, session.ToString()));

                user.LastSession = session;
                await this.db.SaveChangesAsync(cancellationToken);

                string cacheKey = CacheKeyHelper.GetSessionKey(source, user.Id);
                await this.redisCacheService.SetAsync(cacheKey, session);
            }

            var permissionClaims = this.GetClaimPermissions(userPermissions);
            claims.AddRange(permissionClaims);

            var expiredAt = this.GetTokenExpiredAt(type);

            return new() {
                RefreshToken = this.GenerateRefreshToken(claims),
                Token = this.GenerateToken(claims, expiredAt),
                ExpiredTime = new DateTimeOffset(expiredAt).ToUnixTimeMilliseconds(),
                MerchantCode = merchant.Code,
                MerchantName = merchant.Name,
                Username = user.Username,
                Name = user.Name,
                Session = session,
            };
        }

        private DateTime GetTokenExpiredAt(EPermission permissionType) {
            long tokenExpiredAfterDays = this.configuration.GetSection($"TokenExpire:{permissionType}").Get<long?>() ?? 1;
            return DateTimeOffset.Now.AddDays(tokenExpiredAfterDays).Date;
        }

        private string GenerateRefreshToken(List<Claim> claims) {
            var claimsForRefreshToken = new List<Claim>();
            claimsForRefreshToken.AddRange(claims);
            claimsForRefreshToken.Add(new Claim(Constants.TokenRefreshToken, "true"));

            return this.GenerateToken(claimsForRefreshToken, DateTime.Now.AddYears(10));
        }

        private string GenerateToken(List<Claim> claims, DateTime expiredAt) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JwtSecret"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              claims: claims,
              expires: expiredAt,
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private List<Claim> GetClaimPermissions(List<UserPermissionDto> permissions) {
            List<Claim> claims = new();
            foreach (var item in permissions) {
                if (!item.IsEnable) continue;
                if (item.IsClaim)
                    claims.Add(new Claim(ClaimTypes.Role, item.ClaimName));

                if (item.Items != null && item.Items.Any())
                    claims.AddRange(this.GetClaimPermissions(item.Items));
            }
            return claims;
        }
    }
}
