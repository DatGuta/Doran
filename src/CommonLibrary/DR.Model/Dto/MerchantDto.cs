namespace DR.Models.Dto {
    public class MerchantDto {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        [return: NotNullIfNotNull(nameof(entity))]
        public static MerchantDto? FromEntity(Merchant? entity) {
            if (entity == null) return default;

            return new MerchantDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
            };
        }
    }
}
