namespace DR.Models.Dto {
    public class OrderWarehouseRefundDto {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public EWarehouseRefund Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static OrderWarehouseRefundDto? FromEntity(Database.Models.WarehouseRefund? entity) {
            if (entity == null) return default;

            return new OrderWarehouseRefundDto {
                Id = entity.Id,
                Code = entity.Code,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate,
            };
        }
    }
}
