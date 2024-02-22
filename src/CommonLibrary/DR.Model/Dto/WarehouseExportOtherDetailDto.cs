namespace DR.Models.Dto {

    public class WarehouseExportOtherDetailDto {
        public string? Id { get; set; }
        public string WarehouseExportOtherId { get; set; } = string.Empty;
        public string ProductId { get; set; } = null!;
        public decimal Quantity { get; set; }
        public ProductDto? Product { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseExportOtherDetailDto? FromEntity(WarehouseExportOtherDetail? entity) {
            if (entity == null) return default;

            return new() {
                Id = entity.Id,
                WarehouseExportOtherId = entity.WarehouseExportOtherId,
                ProductId = entity.ProductId,
                Quantity = entity.Quantity,
                Product = ProductDto.FromEntity(entity.Product)
            };
        }
    }
}
