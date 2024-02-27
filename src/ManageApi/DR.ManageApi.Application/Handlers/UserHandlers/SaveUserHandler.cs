using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Hashers;
using DR.Contexts.AuditContexts;
using DR.Handlers;
using DR.Helper;
using DR.Models;
using DR.Resource;
using Microsoft.EntityFrameworkCore;

namespace DR.Handlers.UserHandlers {
    public class SaveUserReq : BaseModelReq<UserDto, string> {
        public bool IsSelfUpdate { get; set; }
    }

    public class SaveUserHandler : BaseHandler<SaveUserReq, string> {

        public SaveUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task<string> Handle(SaveUserReq request, CancellationToken cancellationToken) {
            var merchantId = request.MerchantId;
            var userId = request.UserId;
            var model = request.Model;

            if (!request.IsSelfUpdate) {
                await this.Validate(merchantId, model.Role?.Id);
            }

            if (string.IsNullOrWhiteSpace(model.Id)) {
                model.Username = model.Username!.Trim().ToLower();
                return await this.Create(merchantId, userId, model, cancellationToken);
            }
            return await this.Update(merchantId, userId, model, request.IsSelfUpdate, cancellationToken);
        }

        private async Task<string> Create(Guid merchantId, string userId, UserDto model, CancellationToken cancellationToken) {
            var existed = await this.db.Users.AnyAsync(o => o.MerchantId == merchantId && o.Username == model.Username && !o.IsDelete && !o.IsSystem, cancellationToken);
            ManagedException.ThrowIf(existed, Messages.User.CreateOrUpdate.User_Existed);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User.CreateOrUpdate.User_NameIsRequire);

            var user = new Database.Models.User() {
                Id = NGuidHelper.New(),
                MerchantId = merchantId,
                RoleId = model.Role!.Id,
                Username = model.Username!,
                Password = PasswordHasher.Hash(model.Password!),
                PinCode = PasswordHasher.Hash(model.PinCode!),
                Name = model.Name,
                SearchName = StringHelper.UnsignedUnicode(model.Name),
                Phone = model.Phone,
                Province = model.Province?.Code,
                District = model.District?.Code,
                Commune = model.Commune?.Code,
                Address = model.Address,
                IsActive = model.IsActive,
                IsSystem = false,
                IsAdmin = false,
            };
            await this.db.Users.AddAsync(user, cancellationToken);
            await this.db.SaveChangesAsync(cancellationToken);

            await this.mediator.Publish(new UserCAuditContext(merchantId, userId, user.Clone()), cancellationToken);

            return user.Id;
        }

        private async Task<string> Update(Guid merchantId, string userId, UserDto model, bool isSelfUpdate, CancellationToken cancellationToken) {
            var existed = await this.db.Users.AnyAsync(o => o.MerchantId == merchantId && o.Id != model.Id && o.Username == model.Username && !o.IsDelete && !o.IsSystem, cancellationToken);
            ManagedException.ThrowIf(existed, Messages.User.CreateOrUpdate.User_Existed);

            var user = await this.db.Users.FirstOrDefaultAsync(o => o.MerchantId == merchantId && o.Id == model.Id && o.Username == model.Username && !o.IsDelete && !o.IsSystem, cancellationToken);
            ManagedException.ThrowIf(user == null, Messages.User.CreateOrUpdate.User_NotFound);
            ManagedException.ThrowIf(user.IsAdmin && !model.IsActive, Messages.User.CreateOrUpdate.User_NotInactive);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User.CreateOrUpdate.User_NameIsRequire);

            var originUser = user.Clone();

            user.Name = model.Name;
            user.SearchName = StringHelper.UnsignedUnicode(model.Name);
            user.Phone = model.Phone;
            user.Province = model.Province?.Code;
            user.District = model.District?.Code;
            user.Commune = model.Commune?.Code;
            user.Address = model.Address;
            if (!isSelfUpdate) {
                user.RoleId = model.Role!.Id;
                user.IsActive = model.IsActive;
            }

            await this.db.SaveChangesAsync(cancellationToken);

            await this.mediator.Publish(new UserUAuditContext(merchantId, userId, originUser, user.Clone()), cancellationToken);

            return user.Id;
        }

        private async Task Validate(Guid merchantId, string? roleId) {
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(roleId), Messages.User.CreateOrUpdate.Role_NotFound);

            var roleExisted = await this.db.Roles.AnyAsync(o => o.MerchantId == merchantId && o.Id == roleId && !o.IsDelete);
            ManagedException.ThrowIf(!roleExisted, Messages.User.CreateOrUpdate.Role_NotFound);
        }
    }
}
