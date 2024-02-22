namespace DR.Models.Dto {
    public class ProductOnHandOnWarehouseDto {
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal NetWeight { get; set; }
        public decimal OnHand { get; set; }
        public bool IsActive { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static ProductOnHandOnWarehouseDto? FromEntity(ProductOnWarehouse? entity) {
            if (entity == null) return default;

            return new ProductOnHandOnWarehouseDto {
                ProductCode = entity.Product?.Code,
                ProductName = entity.Product?.Name,
                NetWeight = entity.Product?.NetWeight ?? 0,
                OnHand = entity.OnHand,
                IsActive = entity.IsActive,
            };
        }
    }
}
