namespace DR.Models.Dto {
    public class SearchWarehouseExportDto {
        public string Id { get; set; } = null!;
        public string WarehouseExportNo { get; set; } = null!;
        public string? OrderId { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static SearchWarehouseExportDto? FromEntity(WarehouseExport? entity) {
            if (entity == null) return default;

            return new() {
                Id = entity.Id,
                WarehouseExportNo = entity.Code,
                OrderId = entity.OrderId,
            };
        }
    }
}
