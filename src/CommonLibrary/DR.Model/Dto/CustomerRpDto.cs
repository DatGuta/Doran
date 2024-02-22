namespace DR.Models.Dto;

public class CustomerRpDto {
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public decimal OpeningDebt { get; set; }
    public decimal OpeningBalance { get; set; }

    public decimal CurrentDebt { get; set; }
    public decimal CurrentBalance { get; set; }

    public decimal EndDebt { get; set; }
    public decimal EndBalance { get; set; }
}
