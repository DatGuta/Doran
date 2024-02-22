using Newtonsoft.Json;

namespace DR.Models.Dto {

    public class OrderReportData {
        public List<OrderReportTableStore> Items { get; set; } = new();
    }

    public class OrderReportTableStore {
        public string StoreName { get; set; } = null!;
        public int Count => Items?.Sum(o => o.Count) ?? 0;
        public decimal Total => Items?.Sum(o => o.Total) ?? 0;
        public List<OrderReportTableItem> Items { get; set; } = new();
    }

    public class OrderReportTableItem {

        [JsonRequired]
        public DateTimeOffset Date { get; set; }

        public int Count { get; set; }
        public decimal Total { get; set; }
        public List<OrderReportTableChildItem> Items { get; set; } = new();
    }

    public class OrderReportTableChildItem {
        public string Id { get; set; } = null!;

        [JsonIgnore]
        public string StoreId { get; set; } = null!;

        public string OrderNo { get; set; } = null!;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? CustomerName { get; set; }

        public decimal NumberOfProduct { get; set; }
        public decimal Total { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
