namespace DR.Contexts.AuditContexts;

public class CustomerCAuditContext(string merchantId, string userId, Customer entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Customer, entity.Id, entity.Code) {
    public Customer Entity { get; set; } = entity;
}

public class CustomerUAuditContext(string merchantId, string userId, Customer original, Customer actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Customer, original.Id, original.Code) {
    public Customer Original { get; set; } = original;
    public Customer Actual { get; set; } = actual;
}

public class CustomerDAuditContext(string merchantId, string userId, Customer entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Customer, entity.Id, entity.Code) {
    public Customer Entity { get; set; } = entity;
}
