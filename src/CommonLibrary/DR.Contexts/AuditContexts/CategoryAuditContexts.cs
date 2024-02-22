namespace DR.Contexts.AuditContexts {

    public class CategoryCAuditContext(string merchantId, string userId, Category entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Category, entity.Id, entity.Code) {
        public Category Entity { get; set; } = entity;
    }

    public class CategoryUAuditContext(string merchantId, string userId, Category original, Category actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Category, original.Id, original.Code) {
        public Category Original { get; set; } = original;
        public Category Actual { get; set; } = actual;
    }

    public class CategoryDAuditContext(string merchantId, string userId, Category entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Category, entity.Id, entity.Code) {
        public Category Entity { get; set; } = entity;
    }
}
