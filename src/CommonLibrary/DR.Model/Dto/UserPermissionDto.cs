namespace DR.Models {
    public class UserPermissionDto {
        public string Id { get; set; } = string.Empty;
        public string ClaimName { get; set; } = string.Empty;
        public bool IsEnable { get; set; }
        public bool IsClaim { get; set; }

        public List<UserPermissionDto> Items { get; set; } = new List<UserPermissionDto>();

        public static List<UserPermissionDto> MapFromEntities(List<Database.Models.Permission> permissions,
            List<Database.Models.RolePermission>? rolePermissions, bool isAdmin) {
            var items = GetUserPermissions(permissions, isAdmin);

            if (!isAdmin && rolePermissions != null && rolePermissions.Any()) {
                items = IncludeRolePermissions(items, rolePermissions);
            }

            return items;
        }

        private static List<UserPermissionDto> GetUserPermissions(List<Database.Models.Permission> permissions,
            bool isAdmin, string? parentId = null) {
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
            List<Database.Models.RolePermission> rolePermissions, bool isEnable = true) {
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
