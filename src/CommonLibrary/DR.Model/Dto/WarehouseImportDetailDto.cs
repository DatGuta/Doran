namespace DR.Models.Dto {
    public class WarehouseImportDetailDto {
        public string? Id { get; set; }
        public ProductDto? Product { get; set; }

        public decimal SupplierOrderQuantity { get; set; }
        public decimal Quantity { get; set; }

        public decimal Cost { get; set; }
        public decimal SubTotal { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseImportDetailDto? FromEntity(WarehouseImportDetail? entity) {
            if (entity == null) return default;

            return new WarehouseImportDetailDto {
                Id = entity.Id,
                SupplierOrderQuantity = entity.SupplierOrderQuantity,
                Quantity = entity.Quantity,
                Cost = entity.Cost,
                SubTotal = entity.SubTotal,
                Product = ProductDto.FromEntity(entity.Product),
            };
        }
    }
}
