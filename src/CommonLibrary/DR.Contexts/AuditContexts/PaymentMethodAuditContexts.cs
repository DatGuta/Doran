namespace DR.Contexts.AuditContexts {

    public class PaymentMethodCAuditContext(string merchantId, string userId, PaymentMethod entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.PaymentMethod, entity.Id, entity.Code) {
        public PaymentMethod Entity { get; set; } = entity;
    }

    public class PaymentMethodUAuditContext(string merchantId, string userId, PaymentMethod original, PaymentMethod actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.PaymentMethod, original.Id, original.Code) {
        public PaymentMethod Original { get; set; } = original;
        public PaymentMethod Actual { get; set; } = actual;
    }

    public class PaymentMethodDAuditContext(string merchantId, string userId, PaymentMethod entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.PaymentMethod, entity.Id, entity.Code) {
        public PaymentMethod Entity { get; set; } = entity;
    }
}
