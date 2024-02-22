namespace DR.Contexts.AuditContexts {

    public class WhExpOtherCAuditContext(string merchantId, string userId, WarehouseExportOther entity) : AuditContext(merchantId, userId, EAuditAction.Create, EAuditDocType.WarehouseExportOther, entity.Id, entity.Code) {
        public WarehouseExportOther Entity { get; set; } = entity;
    }

    public class WhExpOtherUAuditContext(string merchantId, string userId, WarehouseExportOther original, WarehouseExportOther actual) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.WarehouseExportOther, original.Id, original.Code) {
        public WarehouseExportOther Original { get; set; } = original;
        public WarehouseExportOther Actual { get; set; } = actual;
    }

    public class WhExpOtherDAuditContext(string merchantId, string userId, WarehouseExportOther entity) : AuditContext(merchantId, userId, EAuditAction.Delete, EAuditDocType.WarehouseExportOther, entity.Id, entity.Code) {
        public WarehouseExportOther Entity { get; set; } = entity;
    }
}
