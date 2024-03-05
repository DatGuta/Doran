
using Microsoft.AspNetCore.Authorization;

namespace DR.Attributes {
    public class DrAuthorizeAttribute : AuthorizeAttribute {

        public DrAuthorizeAttribute(params string[] claims) {
            if (claims != null && claims.Length != 0) {
                base.Roles = string.Join(',', claims);
            }
        }
    }

    public static class DrClaim {
        public static class Web {
            public const string Dashboard = "BO.Dashboard";
            public const string Product = "BO.Product";
            public const string Customer = "BO.Customer";
            public const string Order = "BO.Order";
            public const string Order_Update = "BO.Order.Update";
            public const string ReceiptPayment = "BO.ReceiptPayment";
            public const string Supplier = "BO.Supplier";
            public const string Warehouse = "BO.Warehouse";
            public const string Store = "BO.Store";
            public const string Device = "BO.Device";
            public const string Report = "BO.Report";
            public const string Audit = "BO.Audit";
            public const string Setting_General_Order = "BO.General.Order";
            public const string Setting_General_GenerateCode = "BO.General.GenerateCode";
            public const string Setting_General_Template = "BO.General.Template";
            public const string Setting_General_Api = "BO.General.Api";
            public const string Setting_General_Email = "BO.General.Email";
            public const string Setting_User = "BO.User";
            public const string Setting_User_Reset = "BO.User.Reset";
            public const string Setting_Role = "BO.Role";
        }

        public static class Pos {
            public const string Sale = "POS.Sale";
            public const string Order = "POS.Order";
            public const string Setting = "POS.Setting";
        }
    }
}
