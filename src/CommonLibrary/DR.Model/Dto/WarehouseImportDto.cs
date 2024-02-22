namespace DR.Models.Dto {
    public class WarehouseImportDto {
        public string? Id { get; set; }
        public EWarehouseImportType Type { get; set; }
        public SupplierDto? Supplier { get; set; }
        public WarehouseDto? Warehouse { get; set; }

        public EWarehouseImport Status { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? DeliveryCode { get; set; }
        public string? DeliveryBy { get; set; }
        public DateTimeOffset DeliveryDate { get; set; } = DateTimeOffset.UtcNow;

        public UserDto? CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ReceviedDate { get; set; }

        public decimal Total { get; set; }

        public List<WarehouseImportDetailDto> Items { get; set; } = new();

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseImportDto? FromEntity(WarehouseImport? entity,
            UnitResource? unitRes) {
            if (entity == null) return default;

            return new WarehouseImportDto {
                Id = entity.Id,
                Type = entity.Type,
                Status = entity.Status,
                Code = entity.Code,
                Description = entity.Description,
                DeliveryCode = entity.DeliveryCode,
                DeliveryBy = entity.DeliveryBy,
                DeliveryDate = entity.DeliveryDate,
                Supplier = SupplierDto.FromEntity(entity.Supplier),
                Warehouse = WarehouseDto.FromEntity(entity.Warehouse, unitRes),
                CreatedBy = UserDto.FromEntity(entity.CreatedUser, unitRes),
                CreatedDate = entity.CreatedDate,
                ReceviedDate = entity.ReceviedDate,
                Total = entity.Total,
                Items = entity.WarehouseImportDetails?.Select(o => WarehouseImportDetailDto.FromEntity(o)!).ToList() ?? new(),
            };
        }
    }
}
