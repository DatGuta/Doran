using DR.Constant.Enums;

namespace DR.Models.Dto {

    public class DebtDetailDto {
        public string ItemId { get; set; } = string.Empty;
        public EDebtDetailType Type { get; set; }

        public DateTimeOffset Date { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Reason { get; set; }

        public decimal DebtBefore { get; set; }
        public decimal BalanceBefore { get; set; }

        public decimal Debt { get; set; }
        public decimal Balance { get; set; }

        public decimal DebtAfter { get; set; }
        public decimal BalanceAfter { get; set; }
    }
}
