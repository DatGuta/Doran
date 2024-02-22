namespace DR.Contexts.AuditContexts {

    public class WhImpCAuditContext(string merchantId, string userId, WarehouseImport entity) : AuditContext(merchantId, userId, EAuditAction.Create,
              entity.Type == EWarehouseImportType.Supplier ? EAuditDocType.WarehouseImport : EAuditDocType.WarehouseImportOther,
              entity.Id, entity.Code) {
        public WarehouseImport Entity { get; set; } = entity;
    }

    public class WhImpUAuditContext(string merchantId, string userId, WarehouseImport original, WarehouseImport actual) : AuditContext(merchantId, userId, EAuditAction.Update,
              original.Type == EWarehouseImportType.Supplier ? EAuditDocType.WarehouseImport : EAuditDocType.WarehouseImportOther,
              original.Id, original.Code) {
        public WarehouseImport Original { get; set; } = original;
        public WarehouseImport Actual { get; set; } = actual;
    }

    public class WhImpDAuditContext(string merchantId, string userId, WarehouseImport entity) : AuditContext(merchantId, userId, EAuditAction.Delete,
              entity.Type == EWarehouseImportType.Supplier ? EAuditDocType.WarehouseImport : EAuditDocType.WarehouseImportOther,
              entity.Id, entity.Code) {
        public WarehouseImport Entity { get; set; } = entity;
    }
}
