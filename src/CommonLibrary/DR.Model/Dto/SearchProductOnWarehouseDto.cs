using System.Diagnostics.CodeAnalysis;

namespace DR.Models.Dto {
    public class SearchProductOnWarehouseDto {
        public string? WarehouseId { get; set; }
        public decimal OnHand { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static SearchProductOnWarehouseDto? FromEntity(Database.Models.ProductOnWarehouse? entity) {
            if (entity == null) return default;

            return new() {
                WarehouseId = entity.WarehouseId,
                OnHand = entity.OnHand,
            };
        }
    }
}
