namespace DR.Models.Dto {
    public class WarehouseRefundDetailDto {
        public string? Id { get; set; }
        public string? WarehouseExportDetailId { get; set; }
        public ProductDto? Product { get; set; }
        public decimal ExportedQuantity { get; set; }
        public decimal RefundQuantity { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseRefundDetailDto? FromEntity(WarehouseRefundDetail? entity) {
            if (entity == null) return default;

            return new WarehouseRefundDetailDto {
                Id = entity.Id,
                WarehouseExportDetailId = entity.WarehouseExportDetailId,
                Product = ProductDto.FromEntity(entity.Product),
                ExportedQuantity = entity.ExportedQuantity,
                RefundQuantity = entity.RefundQuantity,
            };
        }
    }
}
