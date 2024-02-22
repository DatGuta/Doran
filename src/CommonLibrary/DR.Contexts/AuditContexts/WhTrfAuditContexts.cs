

namespace DR.Contexts.AuditContexts {

    public class WhTrfCAuditContext(string merchantId, string userId, WarehouseTransfer entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.WarehouseTransfer, entity.Id, entity.Code) {
        public WarehouseTransfer Entity { get; set; } = entity;
    }

    public class WhTrfUAuditContext(string merchantId, string userId, WarehouseTransfer original, WarehouseTransfer actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.WarehouseTransfer, original.Id, original.Code) {
        public WarehouseTransfer Original { get; set; } = original;
        public WarehouseTransfer Actual { get; set; } = actual;
    }
}
