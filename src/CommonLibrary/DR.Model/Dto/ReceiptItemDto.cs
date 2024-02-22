namespace DR.Models.Dto {
    public class ReceiptItemDto {
        public string OrderId { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
        public decimal TotalBill { get; set; }
        public decimal OriginRemaining { get; set; }
        public decimal Remaining { get; set; }
        public decimal Value { get; set; }
        public bool HasWarehouseExport { get; set; }
        public bool IsSelected { get; set; }
    }
}
