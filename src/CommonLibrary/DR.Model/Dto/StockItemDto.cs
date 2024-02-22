using Newtonsoft.Json;

namespace DR.Models.Dto {

    public class StockItemDto {
        public string Id { get; set; } = string.Empty;
        public EProductDocumentType DocumentType { get; set; }
        public string DocumentCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal ImportQuantity { get; set; }
        public decimal ExportQuantity { get; set; }
        public decimal OnHandBefore { get; set; }
        public decimal OnHandAfter { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

        [JsonIgnore]
        public string DocumentId { get; set; } = string.Empty;

        public static StockItemDto FromEntity(ProductTracking entity) {
            decimal import = 0;
            decimal export = 0;

            if (entity.Quantity > 0) {
                import = entity.Quantity;
            } else export = -entity.Quantity;

            return new StockItemDto {
                Id = entity.Id,
                DocumentType = entity.DocumentType,
                DocumentId = entity.DocumentId,
                DocumentCode = entity.DocumentCode,
                ImportQuantity = import,
                ExportQuantity = export,
                OnHandBefore = entity.OnHandBefore,
                OnHandAfter = entity.OnHandAfter,
                Date = entity.Date,
            };
        }
    }
}
