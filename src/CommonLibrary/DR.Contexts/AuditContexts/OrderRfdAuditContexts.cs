namespace DR.Contexts.AuditContexts {

    public class OrderRfdCAuditContext(string merchantId, string userId, Order order, OrderAction orderAction) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.OrderRefund, order.Id, order.OrderNo) {
        public Order Order { get; set; } = order;
        public OrderAction OrderAction { get; set; } = orderAction;
    }

    public class OrderRfdUAuditContext(string merchantId, string userId, Order order, OrderAction original, OrderAction actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.OrderRefund, order.Id, order.OrderNo) {
        public Order Order { get; set; } = order;
        public OrderAction Original { get; set; } = original;
        public OrderAction Actual { get; set; } = actual;
    }

    public class OrderRfdDAuditContext(string merchantId, string userId, Order order, OrderAction orderAction) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.OrderRefund, order.Id, order.OrderNo) {
        public Order Order { get; set; } = order;
        public OrderAction OrderAction { get; set; } = orderAction;
    }
}
