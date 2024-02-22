namespace DR.Contexts.AuditContexts {

    public class CusOnCusGrpUAuditContext(string merchantId, string userId) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.CustomerOnCustomerGroup, "", "") {
        public string CustomerGroupCode { get; set; } = null!;
        public List<CusOnCusGrpAuditItem> Items { get; set; } = new();
    }

    public class CusOnCusGrpAuditItem {
        public string CustomerId { get; set; } = null!;
        public string? OldCusGrpId { get; set; }
        public string? NewCusGrpId { get; set; }
    }
}
