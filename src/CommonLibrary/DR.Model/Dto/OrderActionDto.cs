namespace DR.Models.Dto {
    public class OrderActionDto {
        public string Id { get; set; } = string.Empty;
        public EOrderAction Type { get; set; }
        public string Code { get; set; } = string.Empty;
        public int Time { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public bool IsDelete { get; set; }
        public bool HasPayment { get; set; }

        public List<OrderActionDetailDto> Items { get; set; } = new();

        public static OrderActionDto FromEntity(OrderAction entity) {
            return new OrderActionDto {
                Id = entity.Id,
                Type = entity.Type,
                Code = entity.Code,
                Time = entity.Time,
                Date = entity.Date,
                IsDelete = entity.IsDelete,
            };
        }
    }
}
