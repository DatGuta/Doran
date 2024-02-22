namespace DR.Models.Dto {

    public class ProductOnWarehouseStatus {
        public string ProductId { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public bool IsPromotion { get; set; }
        public bool IsOnWarehouse { get; set; }
    }
}
