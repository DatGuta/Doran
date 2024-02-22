namespace DR.Models.Dto {

    public class ProductPurchased {

        public ProductDto? Product { get; set; }
        public decimal Quantity { get; set; } = decimal.Zero;
        public DateTimeOffset LastPurchaseDate { get; set; }
    }
}
