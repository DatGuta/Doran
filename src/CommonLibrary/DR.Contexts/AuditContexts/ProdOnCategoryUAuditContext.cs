namespace DR.Contexts.AuditContexts {

    public class ProdOnCategoryUAuditContext(string merchantId, string userId) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.ProductCategory, "", "") {
        public string CategoryCode { get; set; } = null!;
        public List<string> AddProductIds { get; set; } = new();
        public List<string> DeleteProductIds { get; set; } = new();
    }
}
