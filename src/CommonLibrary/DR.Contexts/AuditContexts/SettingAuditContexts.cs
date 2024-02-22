using DR.Database.ExtendModels;
using Newtonsoft.Json.Linq;

namespace DR.Contexts.AuditContexts {

    public class AdvancedSettingUAuditContext(string merchantId, string userId, AdvancedSettingUAuditItem setting) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.GeneralSetting, "", "") {
        public AdvancedSettingUAuditItem Setting { get; set; } = setting;
    }

    public class AdvancedSettingUAuditItem {
        public ESetting Code { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public NumberFormat Original { get; set; } = null!;
        public NumberFormat Actual { get; set; } = null!;
    }

    public class AutoGenerateUAuditContext(string merchantId, string userId, List<AutoGenerateUAuditItem> settings) : AuditContext(merchantId, userId, EAuditAction.Update, EAuditDocType.GeneralSetting, "", "") {
        public List<AutoGenerateUAuditItem> Settings { get; set; } = settings;
    }

    public class AutoGenerateUAuditItem {
        public ESetting Code { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public JToken Original { get; set; } = null!;
        public JToken Actual { get; set; } = null!;
    }
}
