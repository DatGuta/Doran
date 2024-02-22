namespace DR.Contexts.AuditContexts {

    public class OrderExpCAuditContext(string merchantId, string userId, Order order, OrderAction orderAction) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.OrderExport, order.Id, order.OrderNo) {
        public Order Order { get; set; } = order;
        public OrderAction OrderAction { get; set; } = orderAction;
    }

    public class OrderExpUAuditContext(string merchantId, string userId, Order order, OrderAction original, OrderAction actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.OrderExport, order.Id, order.OrderNo) {
        public Order Order { get; set; } = order;
        public OrderAction Original { get; set; } = original;
        public OrderAction Actual { get; set; } = actual;
    }

    public class OrderExpDAuditContext(string merchantId, string userId, Order order, OrderAction orderAction) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.OrderExport, order.Id, order.OrderNo) {
        public Order Order { get; set; } = order;
        public OrderAction OrderAction { get; set; } = orderAction;
    }
}
