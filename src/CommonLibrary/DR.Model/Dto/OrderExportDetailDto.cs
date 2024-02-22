namespace DR.Models.Dto {
    public class OrderExportDetailDto {
        public string OrderDetailId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal OrderedQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ExportedQuantity { get; set; }
        public decimal Quantity { get; set; }
        public bool IsPromotion { get; set; }
    }
}
