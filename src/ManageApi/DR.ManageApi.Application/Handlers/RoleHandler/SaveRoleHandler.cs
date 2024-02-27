using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Lock;
using DR.CommonServices;
using DR.Constant.Enums;
using DR.Contexts;
using DR.Database.Models;
using DR.Helper;
using DR.Models;
using DR.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DR.Handlers.RoleHandlers {

    public class SaveRoleReq : BaseModelReq<RoleDto, string> { }

    public class SaveRoleHandler : BaseHandler<SaveRoleReq, string> {
        private readonly IAutoGenerationService autoGenerationService;

        public SaveRoleHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.autoGenerationService = serviceProvider.GetRequiredService<IAutoGenerationService>();
        }

        public override async Task<string> Handle(SaveRoleReq request, CancellationToken cancellationToken) {
            var merchantId = request.MerchantId;
            var userId = request.UserId;
            var model = request.Model;
            model.Code = model.Code?.Trim().ToUpper();
            model.Name = model.Name.Trim();

            model.Permissions = await this.ValidatePermissions(model.Permissions, cancellationToken);

            if (string.IsNullOrWhiteSpace(model.Id)) {
                return await Locker.LockByKey($"Merchant:{merchantId}:Role",
                    () => this.Create(merchantId, userId, model, cancellationToken));
            }
            return await this.Update(merchantId, userId, model, cancellationToken);
        }

        private async Task<List<PermissionDto>?> ValidatePermissions(List<PermissionDto>? permissions, CancellationToken cancellationToken) {
            if (permissions == null || !permissions.Any())
                return permissions;

            var activeItems = await this.db.GetPermissions(o => o.IsActive, cancellationToken);
            var activeItemIds = activeItems.Select(o => o.Id).ToList();
            return GetActivePermissions(permissions, activeItemIds);
        }

        private List<PermissionDto> GetActivePermissions(List<PermissionDto> permissions, List<string> activeItemIds, bool parentEnable = true) {
            permissions = permissions.Where(o => activeItemIds.Contains(o.Id)).ToList();

            foreach (var item in permissions) {
                item.IsEnable = parentEnable && item.IsEnable;
                item.Items = this.GetActivePermissions(item.Items, activeItemIds, item.IsEnable);
            }

            return permissions;
        }

        private async Task<string> Create(Guid merchantId, string userId, RoleDto model, CancellationToken cancellationToken) {
            if (string.IsNullOrEmpty(model.Code)) {
                model.Code = await this.autoGenerationService.GenerateCode(merchantId, ESetting.AutoGenerateRoleCode, cancellationToken);
            } else {
                var existed = await this.db.Roles.AnyAsync(o => o.MerchantId == merchantId && o.Code == model.Code, cancellationToken);
                ManagedException.ThrowIf(existed, Messages.Role.CreateOrUpdate.Role_Existed);
            }

            Database.Models.Role entity = new() {
                Id = NGuidHelper.New(),
                MerchantId = merchantId,
                Code = model.Code,
                Name = model.Name,
                SearchName = StringHelper.UnsignedUnicode(model.Name),
            };
            entity.RolePermissions = this.GetRolePermissionEntities(model.Permissions, entity.Id);

            await this.db.Roles.AddAsync(entity, cancellationToken);
            await this.db.SaveChangesAsync(cancellationToken);

            await this.mediator.Publish(new RoleCAuditContext(merchantId, userId, entity), cancellationToken);

            return entity.Id;
        }

        private async Task<string> Update(Guid merchantId, string userId, RoleDto model, CancellationToken cancellationToken) {
            var existed = await this.db.Roles.AsNoTracking()
                .AnyAsync(o => o.MerchantId == merchantId && o.Code == model.Code && o.Id != model.Id && !o.IsDelete, cancellationToken);
            ManagedException.ThrowIf(existed, Messages.Role.CreateOrUpdate.Role_Existed);

            var role = await this.db.Roles.Include(o => o.RolePermissions)
                .FirstOrDefaultAsync(o => o.MerchantId == merchantId && o.Id == model.Id && !o.IsDelete, cancellationToken);
            ManagedException.ThrowIfNull(role, Messages.Role.CreateOrUpdate.Role_NotFound);

            var roleOriginal = role.Clone();

            role.Name = model.Name;
            role.SearchName = StringHelper.UnsignedUnicode(model.Name);

            role.RolePermissions = this.GetRolePermissionEntities(model.Permissions, role.Id);

            await this.db.SaveChangesAsync(cancellationToken);

            await this.mediator.Publish(new RoleUAuditContext(merchantId, userId, roleOriginal, role), cancellationToken);

            return role.Id;
        }

        private List<RolePermission> GetRolePermissionEntities(List<PermissionDto>? permissions, string roleId) {
            List<RolePermission> rolePermissions = new();
            if (permissions != null && permissions.Any()) {
                foreach (var item in permissions) {
                    rolePermissions.Add(new RolePermission {
                        RoleId = roleId,
                        PermissionId = item.Id,
                        IsEnable = item.IsEnable,
                    });
                    rolePermissions.AddRange(this.GetRolePermissionEntities(item.Items, roleId));
                }
            }
            return rolePermissions;
        }
    }
}
