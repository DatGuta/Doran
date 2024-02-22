namespace DR.Models.Dto {

    public class WarehouseAdjustmentDetailDto {
        public string? Id { get; set; }
        public ProductDto? Product { get; set; }

        public decimal OnHand { get; set; }
        public decimal Actual { get; set; }
        public decimal Difference { get; set; }

        public string? Description { get; set; }

        [return: NotNullIfNotNull(nameof(entity))]
        public static WarehouseAdjustmentDetailDto? FromEntity(WarehouseAdjustmentDetail? entity) {
            if (entity == null) return default;

            return new WarehouseAdjustmentDetailDto {
                Id = entity.Id,
                Product = ProductDto.FromEntity(entity.Product),
                OnHand = entity.OnHand,
                Actual = entity.Actual,
                Difference = entity.Difference,
                Description = entity.Description,
            };
        }
    }
}
