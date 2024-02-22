namespace DR.Models.Dto {
    public class OrderExportDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Time { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public bool IsDelete { get; set; }

        public List<OrderExportDetailDto> Items { get; set; } = new();
        public SearchOrderDto? Order { get; set; }

        public decimal TotalBill { get; set; }
    }
}
