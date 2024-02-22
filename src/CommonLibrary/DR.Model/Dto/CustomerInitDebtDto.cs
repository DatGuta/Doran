namespace DR.Models.Dto {

    public class CustomerInitDebtDto {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Debt { get; set; }
    }
}
