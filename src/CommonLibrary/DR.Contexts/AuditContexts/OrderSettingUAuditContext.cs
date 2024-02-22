using Newtonsoft.Json.Linq;

namespace DR.Contexts.AuditContexts {

    public class OrderSettingUAuditContext(string merchantId, string userId, OrderSettingUAuditItem setting) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.GeneralSetting, "", "") {
        public OrderSettingUAuditItem Setting { get; set; } = setting;
    }

    public class OrderSettingUAuditItem {
        public ESetting Code { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public JToken Original { get; set; } = null!;
        public JToken Actual { get; set; } = null!;
    }
}
