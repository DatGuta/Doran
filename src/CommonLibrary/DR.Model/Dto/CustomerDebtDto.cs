namespace DR.Models.Dto {
    public record CustomerDebtDto {
        public decimal AccountAmount { get; set; }
        public decimal CurrentDebt { get; set; }
        public bool DisableReceipt { get; set; }
        public bool DisablePayment { get; set; }
        public bool DisablePaymentRefund { get; set; }
    }
}
