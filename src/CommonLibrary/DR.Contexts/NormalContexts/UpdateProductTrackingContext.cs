namespace DR.Contexts.NormalContexts {
    public record UpdateProductTrackingContext {
        public string MerchantId { get; } = null!;
        public string WarehouseId { get; } = null!;
        public string ProductId { get; } = null!;

        public UpdateProductTrackingContext(string merchantId, string warehouseId, string productId) {
            this.MerchantId = merchantId;
            this.WarehouseId = warehouseId;
            this.ProductId = productId;
        }
    }
}
