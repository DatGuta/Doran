namespace DR.Contexts.AuditContexts {

    public class ProdBrandUAuditContext(string merchantId, string userId) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.ProductBrand, "", "") {
        public string BrandCode { get; set; } = null!;
        public List<string> AddProductIds { get; set; } = new();
        public List<string> DeleteProductIds { get; set; } = new();
    }
}
