namespace DR.Contexts.AuditContexts {

    public class BrandCAuditContext(string merchantId, string userId, Brand entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Brand, entity.Id, entity.Code) {
        public Brand Entity { get; set; } = entity;
    }

    public class BrandUAuditContext(string merchantId, string userId, Brand original, Brand actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Brand, original.Id, original.Code) {
        public Brand Original { get; set; } = original;
        public Brand Actual { get; set; } = actual;
    }

    public class BrandDAuditContext(string merchantId, string userId, Brand entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Brand, entity.Id, entity.Code) {
        public Brand Entity { get; set; } = entity;
    }
}
