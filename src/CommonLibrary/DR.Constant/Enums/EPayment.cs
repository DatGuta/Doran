using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EPayment {

    [Description("Hoàn tiền đơn hàng")]
    Standard = 0,

    [Description("Hoàn tiền khách hàng")]
    Refund = 1,
}
