namespace DR.Models.Dto {
    public record ExportDto {
        public EExport Type { get; set; }
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public bool IsDelete { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public enum EExport {
        Export = 0,
        Other = 1,
    }
}
