namespace DR.Models.Dto {
    public class SuggestPriceDto {
        public string? ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
