namespace DR.Models.Dto {

    public class CategoryDto {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static CategoryDto? FromEntity(Category? entity) {
            if (entity == null) return default;
            return new CategoryDto {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                CreatedDate = entity.CreatedDate,
            };
        }
    }
}
