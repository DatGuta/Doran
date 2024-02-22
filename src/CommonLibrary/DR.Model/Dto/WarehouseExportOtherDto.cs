namespace DR.Models.Dto {

    public class WarehouseExportOtherDto {
        public string? Id { get; set; }
        public string WarehouseId { get; set; } = null!;
        public string? Code { get; set; }
        public DateTimeOffset ExportedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public string? Description { get; set; }
        public bool IsDelete { get; set; }
        public WarehouseDto? Warehouse { get; set; }
        public List<WarehouseExportOtherDetailDto> Items { get; set; } = new();

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseExportOtherDto? FromEntity(Database.Models.WarehouseExportOther? entity, List<WarehouseExportOtherDetail>? details = null) {
            if (entity == null) return default;

            return new WarehouseExportOtherDto() {
                Id = entity.Id,
                Code = entity.Code,
                IsDelete = entity.IsDelete,
                WarehouseId = entity.WarehouseId,
                CreatedDate = entity.CreatedDate,
                Description = entity.Description,
                ExportedDate = entity.ExportedDate,
                Warehouse = WarehouseDto.FromEntity(entity.Warehouse, null),
                Items = details != null ? details.Select(o => WarehouseExportOtherDetailDto.FromEntity(o)).ToList() : new()
            };
        }
    }
}
