namespace DR.Contexts.AuditContexts {

    public class StoreCAuditContext(string merchantId, string userId, Store entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Store, entity.Id, entity.Code) {
        public Store Entity { get; set; } = entity;
    }

    public class StoreUAuditContext(string merchantId, string userId, Store original, Store actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Store, original.Id, original.Code) {
        public Store Original { get; set; } = original;
        public Store Actual { get; set; } = actual;
    }

    public class StoreDAuditContext(string merchantId, string userId, Store entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Store, entity.Id, entity.Code) {
        public Store Entity { get; set; } = entity;
    }
}
