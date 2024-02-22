namespace DR.Contexts.AuditContexts {

    public class PaymentCAuditContext(string merchantId, string userId, Payment entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Payment, entity.Id, entity.Code) {
        public Payment Entity { get; set; } = entity;
    }

    public class PaymentDAuditContext(string merchantId, string userId, Payment entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Payment, entity.Id, entity.Code) {
        public Payment Entity { get; set; } = entity;
    }

    public class PaymentUAuditContext(string merchantId, string userId, Payment original, Payment actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Payment, original.Id, original.Code) {
        public Payment Original { get; set; } = original;
        public Payment Actual { get; set; } = actual;
    }
}
