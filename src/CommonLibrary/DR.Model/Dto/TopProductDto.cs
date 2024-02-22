namespace DR.Models.Dto {
    public class TopProductDto {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Frequency { get; set; }
    }
}
