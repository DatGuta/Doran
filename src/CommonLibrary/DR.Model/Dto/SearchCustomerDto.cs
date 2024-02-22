using System.Diagnostics.CodeAnalysis;

namespace DR.Models.Dto {
    public class SearchCustomerDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [return: NotNullIfNotNull(nameof(entity))]
        public static SearchCustomerDto? FromEntity(Database.Models.Customer? entity) {
            if (entity == null) return default;

            return new() {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
            };
        }
    }
}
