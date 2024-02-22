namespace DR.Models.Dto {
    public class WarehouseExportDetailDto {
        public string? Id { get; set; }
        public string? OrderDetailId { get; set; }
        public ProductDto? Product { get; set; }

        public decimal OrderQuantity { get; set; }
        public decimal ExportedQuantity { get; set; }
        public decimal Quantity { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseExportDetailDto? FromEntity(WarehouseExportDetail? entity,
            Dictionary<string, decimal>? orderedQty = null,
            Dictionary<string, decimal>? exportedQty = null) {
            if (entity == null) return default;
            orderedQty ??= new();
            exportedQty ??= new();

            decimal orderedQtyValue = 0;
            decimal exportedQtyValue = 0;
            if (!string.IsNullOrWhiteSpace(entity.OrderDetailId)) {
                orderedQtyValue = orderedQty.GetValueOrDefault(entity.OrderDetailId);
                exportedQtyValue = exportedQty.GetValueOrDefault(entity.OrderDetailId);
            }

            return new WarehouseExportDetailDto {
                Id = entity.Id,
                OrderDetailId = entity.OrderDetailId,
                Product = ProductDto.FromEntity(entity.Product),
                OrderQuantity = orderedQtyValue,
                ExportedQuantity = exportedQtyValue,
                Quantity = entity.Quantity,
            };
        }
    }
}
