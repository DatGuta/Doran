namespace DR.Common.Models;

public class PtItem {
    public string ProductId { get; set; } = null!;
    public decimal Value { get; set; } = decimal.Zero;
    public DateTimeOffset? Time { get; set; }

    public string TableName { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public bool IsDelete { get; set; }
}
