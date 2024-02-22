namespace DR.Contexts.AuditContexts {

    public class ProductCAuditContext(string merchantId, string userId, Product entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Product, entity.Id, entity.Code) {
        public Product Entity { get; set; } = entity;
    }

    public class ProductUAuditContext(string merchantId, string userId, Product original, Product actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Product, original.Id, original.Code) {
        public Product Original { get; set; } = original;
        public Product Actual { get; set; } = actual;
    }

    public class ProductDAuditContext(string merchantId, string userId, Product entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Product, entity.Id, entity.Code) {
        public Product Entity { get; set; } = entity;
    }
}
