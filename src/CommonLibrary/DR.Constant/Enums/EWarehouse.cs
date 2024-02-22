using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EWarehouse {

    [Description("Kho thường")]
    Normal = 0,

    [Description("Kho phiếu")]
    Ticket = 1,
}
