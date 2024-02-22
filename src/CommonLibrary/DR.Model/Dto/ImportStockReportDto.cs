namespace DR.Models.Dto {

    public class ImportStockReportDto {
        public DateTimeOffset Date { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string TransactionCode { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Weight { get; set; }
        public EProductDocumentType Type { get; set; }
        public string DeliveryBy { get; set; } = string.Empty;
        public List<SupplierDto> Suppliers { get; set; } = new();
    }
}
