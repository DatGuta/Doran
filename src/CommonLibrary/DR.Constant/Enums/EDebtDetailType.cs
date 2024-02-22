using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EDebtDetailType {

    [Description("Đơn hàng")]
    Order = 0,

    [Description("Phiếu thu")]
    Receipt = 1,

    [Description("Phiếu chi")]
    Payment = 2,

    [Description("Trả hàng")]
    Refund = 3
}
