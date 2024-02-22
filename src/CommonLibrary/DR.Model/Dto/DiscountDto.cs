namespace DR.Models.Dto {
    public class DiscountDto {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public EDiscount Type { get; set; }
        public decimal Value { get; set; }
    }
}
