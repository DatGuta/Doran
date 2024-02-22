using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EOrder {

    [Description("Đơn thường")]
    Normal = 0,

    [Description("Đơn phiếu")]
    Ticket = 1,
}
