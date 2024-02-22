namespace DR.Models.Dto {
    public class WarehouseTransferDetailDto {
        public string? Id { get; set; }
        public ProductDto? Product { get; set; }

        public decimal ExportedQuantity { get; set; }
        public decimal ImportQuantity { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseTransferDetailDto? FromEntity(WarehouseTransferDetail? entity) {
            if (entity == null) return default;

            return new WarehouseTransferDetailDto {
                Id = entity.Id,
                ExportedQuantity = entity.ExportedQuantity,
                ImportQuantity = entity.ImportQuantity,
                Product = ProductDto.FromEntity(entity.Product),
            };
        }
    }
}
