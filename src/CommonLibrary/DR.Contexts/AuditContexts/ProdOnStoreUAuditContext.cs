namespace DR.Contexts.AuditContexts {

    public class ProdOnStoreUAuditContext(string merchantId, string userId) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.ProductOnStore, "", "") {
        public string StoreCode { get; set; } = null!;
        public List<string> AddProductIds { get; set; } = new();
        public List<string> DeleteProductIds { get; set; } = new();
    }
}
