namespace DR.Models.Dto {
    public class OrderWarehouseExportDto {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public EWarehouseExport Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static OrderWarehouseExportDto? FromEntity(Database.Models.WarehouseExport? entity) {
            if (entity == null) return default;

            return new OrderWarehouseExportDto {
                Id = entity.Id,
                Code = entity.Code,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate,
            };
        }
    }
}
