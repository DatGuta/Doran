using DR.Database.Models;

namespace DR.Models.Dto {

    public class UserPermissionDto {
        public Guid Id { get; set; }
        public string ClaimName { get; set; } = string.Empty;
        public bool IsEnable { get; set; }
        public bool IsClaim { get; set; }

        public List<UserPermissionDto> Items { get; set; } = [];

        public static List<UserPermissionDto> MapFromEntities(List<Permission> permissions,
            List<RolePermission>? rolePermissions, bool isAdmin) {
            var items = GetUserPermissions(permissions, isAdmin);

            if (!isAdmin && rolePermissions != null && rolePermissions.Count != 0) {
                items = IncludeRolePermissions(items, rolePermissions);
            }

            return items;
        }

        private static List<UserPermissionDto> GetUserPermissions(List<Permission> permissions,
            bool isAdmin, Guid? parentId = null) {
            var permissionDtos = permissions.Where(o => o.IsActive && o.ParentId == parentId).Select(o => new UserPermissionDto {
                Id = o.Id,
                ClaimName = o.ClaimName,
                IsEnable = o.IsDefault || isAdmin,
                IsClaim = o.IsClaim,
            }).ToList();

            permissionDtos.ForEach(o => o.Items = GetUserPermissions(permissions, isAdmin, o.Id));

            return permissionDtos;
        }

        private static List<UserPermissionDto> IncludeRolePermissions(List<UserPermissionDto> permissions,
            List<RolePermission> rolePermissions, bool isEnable = true) {
            foreach (var item in permissions) {
                var rolePermission = rolePermissions.FirstOrDefault(o => o.PermissionId == item.Id);
                if (rolePermission == null) continue;
                item.IsEnable = isEnable && rolePermission.IsEnable;
                item.Items = IncludeRolePermissions(item.Items, rolePermissions, item.IsEnable);
            }
            return permissions;
        }
    }
}
