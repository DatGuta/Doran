namespace DR.Models.Dto {

    public class SearchOrderDto {
        public string Id { get; set; } = null!;
        public string OrderNo { get; set; } = null!;
        public EOrderStatus Status { get; set; }
        public CustomerDto? Customer { get; set; }
        public DeliveryDto? Delivery { get; set; }
        public WarehouseDto? Warehouse { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static SearchOrderDto? FromEntity(Database.Models.Order? entity,
            UnitResource? unitRes) {
            if (entity == null) return default;

            return new() {
                Id = entity.Id,
                OrderNo = entity.OrderNo,
                Status = entity.Status,
                Customer = CustomerDto.FromEntity(entity.Customer, unitRes),
                Warehouse = WarehouseDto.FromEntity(entity.Warehouse, unitRes),
                Delivery = entity.OrderCustomers?.Select(o => DeliveryDto.FromEntity(o, unitRes)).FirstOrDefault(),
            };
        }
    }
}
