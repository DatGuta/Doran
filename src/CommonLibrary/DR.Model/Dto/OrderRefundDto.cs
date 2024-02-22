namespace DR.Models.Dto {
    public class OrderRefundDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Time { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public bool IsDelete { get; set; }
        public bool HasPayment { get; set; }

        public List<OrderRefundDetailDto> Items { get; set; } = new();
        public SearchOrderDto? Order { get; set; }
    }
}
