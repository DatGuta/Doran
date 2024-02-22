using DR.Models.Dto;

namespace DR.Models.SyncDto {
    public class SyncOrderDetailDto {
        public string Id { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public ProductDto Product { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ItemDiscount { get; set; }
        public decimal TotalItem { get; set; }

        public DiscountDto? Discount { get; set; }
    }
}
