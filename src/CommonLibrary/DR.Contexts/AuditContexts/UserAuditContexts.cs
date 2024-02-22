namespace DR.Contexts.AuditContexts {

    public class UserCAuditContext(string merchantId, string userId, User entity) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.User, entity.Id, entity.Username) {
        public User Entity { get; set; } = entity;
    }

    public class UserUAuditContext(string merchantId, string userId, User original, User actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.User, original.Id, original.Username) {
        public User Original { get; set; } = original;
        public User Actual { get; set; } = actual;
    }

    public class UserDAuditContext(string merchantId, string userId, User entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.User, entity.Id, entity.Username) {
        public User Entity { get; set; } = entity;
    }
}
