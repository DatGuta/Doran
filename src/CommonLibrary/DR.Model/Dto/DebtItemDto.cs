namespace DR.Models.Dto {
    public class DebtItemDto {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public EOrderStatus Status { get; set; }

        public DateTimeOffset Time { get; set; }

        public static DebtItemDto FromEntity(
            Database.Models.DebtDetail entity,
            Database.Models.Order? order = null) {
            var item = new DebtItemDto {
                Id = entity.Id,
                Value = entity.Value,
                Time = entity.Time,
            };

            if (order != null) {
                item.OrderId = order.Id;
                item.OrderNo = order.OrderNo;
                item.Status = order.Status;
            }

            return item;
        }
    }
}
