namespace DR.Models.Dto {
    public class StockDto {
        public StockEntityDto Product { get; set; } = new();
        public StockEntityDto Warehouse { get; set; } = new();
        public StockEntityDto? Brand { get; set; }
        public decimal StockBefore { get; set; }
        public decimal StockImport { get; set; }
        public decimal StockExport { get; set; }
        public decimal StockAfter { get; set; }
    }

    public class RawStock {
        public string MerchantId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SearchName { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string? BrandId { get; set; }
        public string? BrandCode { get; set; }
        public string? BrandName { get; set; }
        public decimal StockBefore { get; set; }
        public decimal StockImport { get; set; }
        public decimal StockExport { get; set; }
        public decimal StockAfter { get; set; }
    }
}
