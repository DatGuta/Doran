namespace DR.Contexts.AuditContexts {

    public class ProdOnWhUAuditContext(string merchantId, string userId) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.ProductOnWarehouse, "", "") {
        public string WarehouseCode { get; set; } = null!;
        public List<string> AddProductIds { get; set; } = new();
        public List<string> DeleteProductIds { get; set; } = new();
    }
}
