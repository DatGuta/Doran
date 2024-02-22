namespace DR.Contexts.AuditContexts {

    public class CusGrpCAuditContext(string merchantId, string userId, CustomerGroup entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.CustomerGroup, entity.Id, entity.Code) {
        public CustomerGroup Entity { get; set; } = entity;
    }

    public class CusGrpUAuditContext(string merchantId, string userId, CustomerGroup original, CustomerGroup actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.CustomerGroup, original.Id, original.Code) {
        public CustomerGroup Original { get; set; } = original;
        public CustomerGroup Actual { get; set; } = actual;
    }

    public class CusGrpDAuditContext(string merchantId, string userId, CustomerGroup entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.CustomerGroup, entity.Id, entity.Code) {
        public CustomerGroup Entity { get; set; } = entity;
    }
}
