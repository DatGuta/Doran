using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EOrderStatus {

    [Description("Đơn tạm")]
    Draft = 0,

    [Description("Đơn mới")]
    New = 1,

    [Description("Đơn phiếu")]
    Ticket = 2,

    [Description("Xuất một phần")]
    Export = 4,

    [Description("Đã xuất")]
    Exported = 5,

    [Description("Đã huỷ")]
    Void = 9,
}
