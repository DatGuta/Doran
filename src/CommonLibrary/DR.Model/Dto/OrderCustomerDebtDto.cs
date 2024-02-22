namespace DR.Models.Dto {
    public class OrderCustomerDebtDto {
        public string Id { get; set; } = null!;
        public string OrderNo { get; set; } = null!;
        public EOrderStatus Status { get; set; }
        public EOrderPaymentStatus PaymentStatus { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public decimal TotalBill { get; set; }
        public decimal Remaining { get; set; }
        public bool HasWarehouseExport { get; set; }

        public static OrderCustomerDebtDto FromEntity(Order order, List<string> orderIdsHasExport) {
            return new OrderCustomerDebtDto {
                Id = order.Id,
                OrderNo = order.OrderNo,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                CreatedDate = order.CreatedDate,
                TotalBill = order.TotalBill,
                Remaining = order.Remaining,
                HasWarehouseExport = orderIdsHasExport.Contains(order.Id),
            };
        }
    }
}
