using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EPaymentMethodType {

    [Description("Tiền mặt")]
    Cash = 0,

    [Description("Chuyển khoản")]
    Banking = 1,
}
