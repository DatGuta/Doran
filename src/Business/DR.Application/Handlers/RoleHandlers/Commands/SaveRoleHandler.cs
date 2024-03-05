using DR.Application.Handlers.AuthHandlers;
using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Lock;
using DR.Constant;
using DR.Database.Models;
using DR.Helper;
using DR.Models.Dto;

namespace DR.Application.Handlers.RoleHandlers.Commands;

public class SaveRoleCommand : ModelRequest<RoleDto, Guid> { }

public class SaveRoleHandler(IServiceProvider serviceProvider) : BaseHandler<SaveRoleCommand, Guid>(serviceProvider) {
    public override async Task<Guid> Handle(SaveRoleCommand request, CancellationToken cancellationToken) {
        var userId = request.UserId;
        var model = request.Model;
        model.Code = model.Code?.Trim().ToUpper();
        model.Name = model.Name.Trim();

        model.Permissions = await this.ValidatePermissions(model.Permissions, cancellationToken);

        if (model.Id.IfNullOrEmpty()) {
            return await Locker.LockByKey($"Role",
                () => this.Create(userId, model, cancellationToken));
        }
        return await this.Update(userId, model, cancellationToken);
    }

    private async Task<List<PermissionDto>?> ValidatePermissions(List<PermissionDto>? permissions, CancellationToken cancellationToken) {
        if (permissions == null || permissions.Count == 0)
            return permissions;

        var activeItems = await this.db.GetPermissions(o => o.IsActive, cancellationToken);
        var activeItemIds = activeItems.Select(o => o.Id).ToList();
        return GetActivePermissions(permissions, activeItemIds);
    }

    private List<PermissionDto> GetActivePermissions(List<PermissionDto> permissions, List<Guid> activeItemIds, bool parentEnable = true) {
        permissions = permissions.Where(o => activeItemIds.Contains(o.Id)).ToList();

        foreach (var item in permissions) {
            item.IsEnable = parentEnable && item.IsEnable;
            item.Items = this.GetActivePermissions(item.Items, activeItemIds, item.IsEnable);
        }

        return permissions;
    }

    private async Task<Guid> Create(Guid userId, RoleDto model, CancellationToken cancellationToken) {
        var existed = await this.db.Roles.AnyAsync(o => o.Code == model.Code, cancellationToken);
        ManagedException.ThrowIf(model.Id.IfNullOrEmpty() || existed, Messages.Role_Existed);

        Role entity = new() {
            Id = Guid.NewGuid(),
            Code = model.Code!,
            Name = model.Name,
            SearchName = StringHelper.UnsignedUnicode(model.Name),
        };
        entity.RolePermissions = GetRolePermissionEntities(model.Permissions, entity.Id);

        await this.db.Roles.AddAsync(entity, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private async Task<Guid> Update(Guid userId, RoleDto model, CancellationToken cancellationToken) {
        var existed = await this.db.Roles.AsNoTracking()
            .AnyAsync(o => o.Code == model.Code && o.Id != model.Id && !o.IsDelete, cancellationToken);
        ManagedException.ThrowIf(existed, Messages.Role_Existed);

        var role = await this.db.Roles.Include(o => o.RolePermissions)
            .FirstOrDefaultAsync(o => o.Id == model.Id && !o.IsDelete, cancellationToken);
        ManagedException.ThrowIfNull(role, Messages.Role_NotFound);

        var roleOriginal = role.Clone();

        role.Name = model.Name;
        role.SearchName = StringHelper.UnsignedUnicode(model.Name);

        role.RolePermissions = GetRolePermissionEntities(model.Permissions, role.Id);
        await this.db.SaveChangesAsync(cancellationToken);
        return role.Id;
    }

    private static List<RolePermission> GetRolePermissionEntities(List<PermissionDto>? permissions, Guid roleId) {
        List<RolePermission> rolePermissions = [];
        if (permissions != null && permissions.Count != 0) {
            foreach (var item in permissions) {
                rolePermissions.Add(new RolePermission {
                    RoleId = roleId,
                    PermissionId = item.Id,
                    IsEnable = item.IsEnable,
                });
                rolePermissions.AddRange(GetRolePermissionEntities(item.Items, roleId));
            }
        }
        return rolePermissions;
    }
}
