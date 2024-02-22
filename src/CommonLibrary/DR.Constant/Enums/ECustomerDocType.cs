using System.ComponentModel;

namespace DR.Constant.Enums;

public enum ECustomerDocType {

    [Description("Nợ gốc")]
    Init = 0,

    [Description("Đơn hàng")]
    Order = 1,

    [Description("Trả hàng")]
    OrderRefund = 2,

    [Description("Phiếu thu")]
    Receipt = 3,

    [Description("Phiếu chi")]
    PaymentRefund = 4,

    [Description("Phiếu chi")]
    PaymentStandard = 5,
}
