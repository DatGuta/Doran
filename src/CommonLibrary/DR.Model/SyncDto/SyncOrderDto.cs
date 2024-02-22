using DR.Models.Dto;

namespace DR.Models.SyncDto {
    public class SyncOrderDto {
        public string Id { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public EOrderStatus Status { get; set; } = EOrderStatus.Draft;
        public decimal SubTotal { get; set; }
        public decimal BillDiscount { get; set; }
        public decimal SubDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalBill { get; set; }

        public EOrderPaymentStatus PaymentStatus { get; set; } = EOrderPaymentStatus.Unpaid;
        public decimal Remaining { get; set; }
        public decimal Paid { get; set; }

        public string? Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public List<SyncOrderDetailDto> Items { get; set; } = null!;

        public StoreDto? Store { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public CustomerDto? Customer { get; set; }

        public DiscountDto? Discount { get; set; }
    }
}
