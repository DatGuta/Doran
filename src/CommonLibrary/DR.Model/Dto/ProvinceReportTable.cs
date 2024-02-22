using Newtonsoft.Json;

namespace DR.Models.Dto {

    public class ProvinceReportTable {
        public string Province { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public string CustomerName { get; set; } = string.Empty;
        public decimal Count => Items?.Sum(o => o.Count) ?? default;
        public long Total => Items?.Sum(o => o.Total) ?? default;
        public List<ProvinceReportItem> Items { get; set; } = new();
    }

    public class ProvinceReportItem {
        public string ProductId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal Count { get; set; }
        public long Total { get; set; }

        [JsonIgnore]
        public List<string> OrderDetailIds = new();
    }
}
