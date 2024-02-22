namespace DR.Contexts.AuditContexts {

    public class ReceiptCAuditContext(string merchantId, string userId, Receipt entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Receipt, entity.Id, entity.Code) {
        public Receipt Entity { get; set; } = entity;
    }

    public class ReceiptUAuditContext(string merchantId, string userId, Receipt original, Receipt actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Receipt, original.Id, original.Code) {
        public Receipt Original { get; set; } = original;
        public Receipt Actual { get; set; } = actual;
    }

    public class ReceiptDAuditContext(string merchantId, string userId, Receipt entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Receipt, entity.Id, entity.Code) {
        public Receipt Entity { get; set; } = entity;
    }
}
