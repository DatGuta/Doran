using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Constant;
using DR.Constant.Enums;
using DR.Helper;
using DR.Models.Dto;
using DR.Redis.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DR.Application.Handlers.AuthHandlers.Commands;

public class LoginCommand : IRequest<LoginResult> {
    public string MerchantCode { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public EPermission Permission { get; set; }
}

public class LoginResult {
    public string RefreshToken { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long ExpiredTime { get; set; }
    public long Session { get; set; }
}

public class LoginHandler(IServiceProvider serviceProvider) : BaseHandler<LoginCommand, LoginResult>(serviceProvider) {
    private readonly IRedisService redisCacheService = serviceProvider.GetRequiredService<IRedisService>();

    public override async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken) {
        request.Username = request.Username.ToLower().Trim();
        return await this.Login(request, request.Permission, cancellationToken);
    }

    public async Task<LoginResult> Login(LoginCommand request, EPermission type, CancellationToken cancellationToken) {
        var user = await this.db.Users.FirstOrDefaultAsync(o => o.Username == request.Username, cancellationToken);
        ManagedException.ThrowIfNull(user, Messages.User_NotFound);
        ManagedException.ThrowIf(!PasswordHasher.Verify(request.Password, user.Password), Messages.User_IncorrectUserNameOrPassword);

        var permissions = await this.db.GetPermissions(o => o.Type == type, cancellationToken);
        Database.Models.Role? role = null;
        if (user.RoleId.HasValue) {
            role = await db.Roles.Include(o => o.RolePermissions).AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == user.RoleId, cancellationToken);
        }

        var userPermissions = UserPermissionDto.MapFromEntities(permissions, role?.RolePermissions?.ToList(), user.IsAdmin);
        ManagedException.ThrowIfFalse(userPermissions.Exists(o => o.IsEnable), Messages.User_NoPermission);

        var claims = new List<Claim>() {
            new(Constants.TokenUserId, user.Id.ToString()),
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

        var permissionClaims = GetClaimPermissions(userPermissions);
        claims.AddRange(permissionClaims);

        var expiredAt = this.GetTokenExpiredAt(type);

        return new() {
            RefreshToken = this.GenerateRefreshToken(claims),
            Token = this.GenerateToken(claims, expiredAt),
            ExpiredTime = new DateTimeOffset(expiredAt).ToUnixTimeMilliseconds(),
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

    private static List<Claim> GetClaimPermissions(List<UserPermissionDto> permissions) {
        List<Claim> claims = [];
        foreach (var item in permissions) {
            if (!item.IsEnable) continue;
            if (item.IsClaim)
                claims.Add(new Claim(ClaimTypes.Role, item.ClaimName));

            if (item.Items != null && item.Items.Count != 0)
                claims.AddRange(GetClaimPermissions(item.Items));
        }
        return claims;
    }
}
