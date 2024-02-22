namespace DR.Models.Dto {
    public class WarehouseRefundDto {
        public string? Id { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public SearchOrderDto? Order { get; set; }
        public SearchWarehouseExportDto? WarehouseExport { get; set; }

        public EWarehouseRefund Status { get; set; }
        public string? Code { get; set; }

        public UserDto? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? RefundDate { get; set; }
        public string? Description { get; set; }

        public List<WarehouseRefundDetailDto> Items { get; set; } = new();

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseRefundDto? FromEntity(WarehouseRefund? entity,
            UnitResource unitRes) {
            if (entity == null) return default;

            return new WarehouseRefundDto {
                Id = entity.Id,
                Warehouse = WarehouseDto.FromEntity(entity.Warehouse, unitRes),
                Order = SearchOrderDto.FromEntity(entity.Order, unitRes),
                WarehouseExport = SearchWarehouseExportDto.FromEntity(entity.WarehouseExport),
                Status = entity.Status,
                Code = entity.Code,
                CreatedBy = UserDto.FromEntity(entity.CreatedUser, unitRes),
                CreatedDate = entity.CreatedDate,
                RefundDate = entity.RefundDate,
                Description = entity.Description,
                Items = entity.WarehouseRefundDetails?.Select(o => WarehouseRefundDetailDto.FromEntity(o)!).ToList() ?? new(),
            };
        }
    }
}
