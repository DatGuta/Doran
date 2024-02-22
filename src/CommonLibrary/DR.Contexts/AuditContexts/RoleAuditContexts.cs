namespace DR.Contexts.AuditContexts {

    public class RoleCAuditContext(string merchantId, string userId, Role entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Role, entity.Id, entity.Code) {
        public Role Entity { get; set; } = entity;
    }

    public class RoleUAuditContext(string merchantId, string userId, Role original, Role actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Role, original.Id, original.Code) {
        public Role Original { get; set; } = original;
        public Role Actual { get; set; } = actual;
    }

    public class RoleDAuditContext(string merchantId, string userId, Role entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Role, entity.Id, entity.Code) {
        public Role Entity { get; set; } = entity;
    }
}
