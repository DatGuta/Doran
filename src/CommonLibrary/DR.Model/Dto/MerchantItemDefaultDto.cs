namespace DR.Models.Dto {

    public class MerchantItemDefaultDto {
        public string Id { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public string? ItemId { get; set; }
        public EMerchantItemDefault Type { get; set; }
    }
}
