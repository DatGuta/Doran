using Newtonsoft.Json;

namespace DR.Models.Dto {
    public class UserDto {
        public string? Id { get; set; }
        public string? Username { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Password { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? PinCode { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public Unit? Province { get; set; }
        public Unit? District { get; set; }
        public Unit? Commune { get; set; }
        public string? Address { get; set; }

        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

        public RoleDto? Role { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static UserDto? FromEntity(Database.Models.User? entity,
            UnitResource? unitRes,
            Database.Models.Role? roleEntity = null) {
            if (entity == null) return default;
            entity.Role ??= roleEntity;

            var au = unitRes?.GetByCode(entity.Province, entity.District, entity.Commune) ?? new();

            return new UserDto {
                Id = entity.Id,
                Username = entity.Username,
                Name = entity.Name,
                Phone = entity.Phone,
                Province = !string.IsNullOrWhiteSpace(entity.Province) && au.TryGetValue(entity.Province, out var province) ? province : default,
                District = !string.IsNullOrWhiteSpace(entity.District) && au.TryGetValue(entity.District, out var district) ? district : default,
                Commune = !string.IsNullOrWhiteSpace(entity.Commune) && au.TryGetValue(entity.Commune, out var commune) ? commune : default,
                Address = entity.Address,
                IsActive = entity.IsActive,
                IsAdmin = entity.IsAdmin,
                Role = RoleDto.FromEntity(entity.Role),
            };
        }
    }
}
