using System.Diagnostics.CodeAnalysis;

namespace DR.Models.Dto {
    public class ProductOnWasehouseDto {
        public WarehouseDto? Warehouse { get; set; }
        public decimal OnHand { get; set; }
        public bool IsActive { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static ProductOnWasehouseDto? FromEntity(Database.Models.ProductOnWarehouse? entity, Database.Models.Warehouse? warehouse = null) {
            if (entity == null) return default;

            return new ProductOnWasehouseDto {
                Warehouse = WarehouseDto.FromEntity(entity.Warehouse ?? warehouse, null),
                OnHand = entity.OnHand,
                IsActive = entity.IsActive,
            };
        }
    }
}
