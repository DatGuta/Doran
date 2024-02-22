namespace DR.Models.Dto {
    public class CustomerReportByOrderTable {
        public string CustomerId { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public int Count => Items?.Sum(o => o.Count) ?? default;
        public decimal Total => Items?.Sum(o => o.Total) ?? default;
        public List<CustomerReportByOrderTableItem> Items { get; set; } = new();
    }

    public class CustomerReportByOrderTableItem {
        public DateTimeOffset Date { get; set; }
        public int Count => Items?.Count ?? default;
        public decimal Total => Items?.Sum(o => o.Total) ?? default;
        public List<CustomerReportByOrderTableChildItem> Items { get; set; } = new();
    }

    public class CustomerReportByOrderTableChildItem {
        public string Id { get; set; } = null!;
        public string OrderNo { get; set; } = null!;
        public EOrderStatus Status { get; set; }
        public decimal NumberOfProduct { get; set; }
        public decimal Total { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
