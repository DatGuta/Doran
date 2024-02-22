using Newtonsoft.Json;

namespace DR.Models.Dto {

    public class CustomerReportByProductTable {
        public string CustomerId { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal Count => Items?.Sum(o => o.Count) ?? default;
        public decimal Exported => Items?.Sum(o => o.Exported) ?? default;
        public decimal Refunded => Items?.Sum(o => o.Refunded) ?? default;
        public long Total => Items?.Sum(o => o.Total) ?? default;
        public List<CustomerReportByProductTableItem> Items { get; set; } = new();
    }

    public class CustomerReportByProductTableItem {
        public string ProductId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Count => ChildItems?.Sum(o => o.ItemCount) ?? default;
        public decimal Exported => ChildItems?.Sum(o => o.ItemExported) ?? default;
        public decimal Refunded => ChildItems?.Sum(o => o.ItemRefunded) ?? default;
        public long Total => ChildItems?.Sum(o => o.ItemTotal) ?? default;
        public List<CustomerReportByProductTableChildItem> ChildItems { get; set; } = new();

        [JsonIgnore]
        public List<string> OrderDetailIds { get; set; } = new();
    }

    public class CustomerReportByProductTableChildItem {
        public DateTime CreateDate { get; set; }
        public decimal ItemCount { get; set; }
        public decimal ItemExported { get; set; }
        public decimal ItemRefunded { get; set; }
        public long ItemTotal { get; set; }
    }
}
