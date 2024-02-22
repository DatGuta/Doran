namespace DR.Models.Dto {
    public class ProductOnStoreDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsOnStore { get; set; }
    }
}
