namespace DR.Models.Dto {
    public class OrderActionDetailDto {
        public string OrderDetailId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public static OrderActionDetailDto FromEntity(OrderActionDetail entity) {
            return new OrderActionDetailDto {
                OrderDetailId = entity.OrderDetailId,
                Quantity = entity.Quantity,
                Price = entity.Price,
            };
        }
    }
}
