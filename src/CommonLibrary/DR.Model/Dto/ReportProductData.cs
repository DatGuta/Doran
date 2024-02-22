using System.Text.Json.Serialization;

namespace DR.Models.Dto {

    public class ProductReportTable {
        public string ProductId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal ProductWeight { get; set; }
        public decimal Ordered => Items?.Sum(o => o.Ordered) ?? 0;
        public decimal Exported => Items?.Sum(o => o.Exported) ?? 0;
        public decimal Refunded => Items?.Sum(o => o.Refunded) ?? 0;
        public decimal Total => Items?.Sum(o => o.Total) ?? 0;
        public List<ProductReportTableItem> Items { get; set; } = new();
    }

    public class ProductReportTableItem {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Ordered => Items?.Sum(o => o.Ordered) ?? 0;
        public decimal Exported => Items?.Sum(o => o.Exported) ?? 0;
        public decimal Refunded => Items?.Sum(o => o.Refunded) ?? 0;
        public decimal Total => Items?.Sum(o => o.Total) ?? 0;
        public List<ProductReportChildItem> Items { get; set; } = new();
    }

    public class ProductReportChildItem {
        public string CustomerId { get; set; } = null!;
        public string CustomerCode { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public string OrderNo { get; set; } = null!;
        public decimal Ordered { get; set; }
        public decimal Exported { get; set; }
        public decimal Refunded { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public DateTimeOffset CreateDate { get; set; }

        [JsonIgnore]
        public string CustomerProviceCode { get; set; } = string.Empty;
        [JsonIgnore]
        public string CustomerProvice { get; set; } = string.Empty;
        [JsonIgnore]
        public string CustomerDistrict { get; set; } = string.Empty;
        [JsonIgnore]
        public string CustomerCommune { get; set; } = string.Empty;
    }
}
