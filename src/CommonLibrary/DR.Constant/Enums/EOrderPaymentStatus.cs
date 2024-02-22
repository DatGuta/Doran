using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EOrderPaymentStatus {

    [Description("Chưa thanh toán")]
    Unpaid = 0,

    [Description("Thanh toán một phần")]
    PartialPaid = 1,

    [Description("Đã thanh toán")]
    Paid = 2,
}
