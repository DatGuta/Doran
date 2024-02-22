namespace DR.Models.Dto;

public class SaleChartDto {
    public string CustomerCode { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal ProductQuantity { get; set; }
}
