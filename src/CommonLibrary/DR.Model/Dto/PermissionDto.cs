using Newtonsoft.Json;

namespace DR.Models.Dto {
    public class PermissionDto {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsEnable { get; set; }
        public int OrderIndex { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<PermissionDto> Items { get; set; } = new();

        public static List<PermissionDto> FromEntities(List<Database.Models.Permission> permissions, string? parentId = null) {
            var permissionDtos = permissions.Where(o => o.IsActive && o.ParentId == parentId).Select(o => new PermissionDto {
                Id = o.Id,
                Name = o.DisplayName,
                IsEnable = o.IsDefault,
                OrderIndex = o.OrderIndex,
            }).OrderBy(o => o.OrderIndex).ToList();

            permissionDtos.ForEach(o => o.Items = FromEntities(permissions, o.Id));

            return permissionDtos;
        }
    }
}
