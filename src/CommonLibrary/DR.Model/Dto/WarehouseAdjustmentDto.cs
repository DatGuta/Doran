namespace DR.Models.Dto {
    public class WarehouseAdjustmentDto {
        public string? Id { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public bool IsAdjust { get; set; }
        public bool IsDelete { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public UserDto? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public string? Reporter { get; set; }
        public DateTimeOffset ReportAt { get; set; } = DateTimeOffset.UtcNow;
        public List<WarehouseAdjustmentDetailDto> Items { get; set; } = new();

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseAdjustmentDto? FromEntity(WarehouseAdjustment? entity,
            UnitResource? unitRes) {
            if (entity == null) return default;

            return new WarehouseAdjustmentDto {
                Id = entity.Id,
                Warehouse = WarehouseDto.FromEntity(entity.Warehouse, unitRes),
                IsAdjust = entity.IsAdjust,
                IsDelete = entity.IsDelete,
                Code = entity.Code,
                CreatedBy = UserDto.FromEntity(entity.User, unitRes, null),
                CreatedDate = entity.CreatedDate,
                Description = entity.Description,
                Reporter = entity.Reporter,
                ReportAt = entity.ReportAt,
                Items = entity.WarehouseAdjustmentDetails?.Select(o => WarehouseAdjustmentDetailDto.FromEntity(o)!).ToList() ?? new(),
            };
        }
    }
}
