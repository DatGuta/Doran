namespace DR.Models.Dto {

    public class PaymentItemDto {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public decimal RefundQuantity { get; set; }
        public decimal RefundTotal { get; set; }
        public decimal Value { get; set; }
        public decimal Remaining { get; set; }
        public bool IsSelected { get; set; }

        public static PaymentItemDto FormEntity(Database.Models.PaymentDetail entity, Database.Models.Order order) {
            return new PaymentItemDto() {
                Id = entity.Id,
                OrderId = entity.OrderId!,
                OrderNo = order.OrderNo,
                CreatedDate = order.CreatedDate,
                RefundQuantity = order.TotalRefundQuantity,
                RefundTotal = order.TotalRefund,
                Value = entity.Value,
                Remaining = 0,
            };
        }
    }
}
