namespace DR.Contexts.AuditContexts {

    public class CustomerInitDebtCAuditContext(string merchantId, string userId, Customer customer, decimal debt) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.CustomerDebt, string.Empty, string.Empty) {
        public Customer Customer { get; set; } = customer;
        public decimal Debt { get; set; } = debt;
    }

    public class CustomerInitDebtUAuditContext(string merchantId, string userId, Customer customer, decimal original, decimal actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.CustomerDebt, string.Empty, string.Empty) {
        public Customer Customer { get; set; } = customer;
        public decimal OriginalDebt { get; set; } = original;
        public decimal ActualDebt { get; set; } = actual;
    }
}
