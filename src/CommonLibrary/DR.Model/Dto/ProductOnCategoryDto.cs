namespace DR.Models.Dto {
    public class ProductOnCategoryDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsPromotion { get; set; }
        public bool IsOnCategory { get; set; }
    }
}
