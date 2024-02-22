namespace DR.Contexts.AuditContexts {

    public class OrderCAuditContext(string merchantId, string userId, Order entity, ESource source) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Order, entity.Id, entity.OrderNo) {
        public Order Entity { get; set; } = entity;
        public ESource Source { get; set; } = source;
    }

    public class OrderUAuditContext(string merchantId, string userId, Order original, Order actual, ESource source) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Order, original.Id, original.OrderNo) {
        public Order Original { get; set; } = original;
        public Order Actual { get; set; } = actual;
        public ESource Source { get; set; } = source;
    }

    public class OrderDAuditContext(string merchantId, string userId, Order entity, ESource source) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Order, entity.Id, entity.OrderNo) {
        public Order Entity { get; set; } = entity;
        public ESource Source { get; set; } = source;
    }
}
