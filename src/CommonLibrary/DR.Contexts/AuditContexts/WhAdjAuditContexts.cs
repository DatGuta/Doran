namespace DR.Contexts.AuditContexts {

    public class WhAdjCAuditContext(string merchantId, string userId, WarehouseAdjustment entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.WarehouseAdjustment, entity.Id, entity.Code) {
        public WarehouseAdjustment Entity { get; set; } = entity;
    }

    public class WhAdjUAuditContext(string merchantId, string userId, WarehouseAdjustment original, WarehouseAdjustment actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.WarehouseAdjustment, original.Id, original.Code) {
        public WarehouseAdjustment Original { get; set; } = original;
        public WarehouseAdjustment Actual { get; set; } = actual;
    }

    public class WhAdjDAuditContext(string merchantId, string userId, WarehouseAdjustment entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.WarehouseAdjustment, entity.Id, entity.Code) {
        public WarehouseAdjustment Entity { get; set; } = entity;
    }
}
