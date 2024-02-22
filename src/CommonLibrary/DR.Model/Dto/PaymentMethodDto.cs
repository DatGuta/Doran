namespace DR.Models.Dto {

    public class PaymentMethodDto {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public EPaymentMethodType Type { get; set; }
        public bool IsDefault { get; set; } = false;

        [return: NotNullIfNotNull(nameof(entity))]
        public static PaymentMethodDto? FromEntity(Database.Models.PaymentMethod? entity) {
            if (entity == null) return default;

            return new PaymentMethodDto {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Type = entity.Type,
                IsDefault = entity.IsDefault,
            };
        }
    }
}
