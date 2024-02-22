using System.Diagnostics.CodeAnalysis;

namespace DR.Models.Dto {

    public class OrderPaymentDto {
        public string? Id { get; set; }
        public string OrderId { get; set; } = null!;
        public PaymentMethodDto? PaymentMethod { get; set; }
        public decimal Value { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public bool IsDefault { get; set; } = false;

        [return: NotNullIfNotNull(nameof(entity))]
        public static OrderPaymentDto? FromEntity(Database.Models.OrderPayment? entity) {
            if (entity == null) return default;

            return new OrderPaymentDto {
                Id = entity.Id,
                OrderId = entity.OrderId,
                PaymentMethod = PaymentMethodDto.FromEntity(entity.PaymentMethod),
                Value = entity.Value,
                Description = entity.Description,
                CreatedDate = entity.CreatedDate,
            };
        }
    }
}
