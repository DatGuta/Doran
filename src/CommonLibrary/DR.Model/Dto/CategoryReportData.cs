namespace DR.Models.Dto {

    public class CategoryReportTable {
        public string CategoryId { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public decimal OnHand => Items?.Sum(o => o.OnHand) ?? 0;
        public decimal Quantity => Items?.Sum(o => o.Quantity) ?? 0;
        public decimal Refund => Items?.Sum(o => o.Refund) ?? 0;
        public decimal Total => Items?.Sum(o => o.Total) ?? 0;
        public List<CategoryReportTableItem> Items { get; set; } = new();
    }

    public class CategoryReportTableItem {
        public string ProductId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal OnHand { get; set; }
        public decimal Quantity { get; set; }
        public decimal Refund { get; set; }
        public decimal Total { get; set; }
    }
}
