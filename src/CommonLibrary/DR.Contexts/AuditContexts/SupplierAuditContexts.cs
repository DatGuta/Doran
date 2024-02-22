namespace DR.Contexts.AuditContexts {

    public class SupplierCAuditContext(string merchantId, string userId, Supplier entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Supplier, entity.Id, entity.Code) {
        public Supplier Entity { get; set; } = entity;
    }

    public class SupplierUAuditContext(string merchantId, string userId, Supplier original, Supplier actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Supplier, original.Id, original.Code) {
        public Supplier Original { get; set; } = original;
        public Supplier Actual { get; set; } = actual;
    }

    public class SupplierDAuditContext(string merchantId, string userId, Supplier entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Supplier, entity.Id, entity.Code) {
        public Supplier Entity { get; set; } = entity;
    }
}
