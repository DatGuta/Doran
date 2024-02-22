namespace DR.Contexts.AuditContexts {

    public class WarehouseCAuditContext(string merchantId, string userId, Warehouse entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.Warehouse, entity.Id, entity.Code) {
        public Warehouse Entity { get; set; } = entity;
    }

    public class WarehouseUAuditContext(string merchantId, string userId, Warehouse original, Warehouse actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.Warehouse, original.Id, original.Code) {
        public Warehouse Original { get; set; } = original;
        public Warehouse Actual { get; set; } = actual;
    }

    public class WarehouseDAuditContext(string merchantId, string userId, Warehouse entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.Warehouse, entity.Id, entity.Code) {
        public Warehouse Entity { get; set; } = entity;
    }
}
