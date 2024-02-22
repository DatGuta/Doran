using System.ComponentModel;

namespace DR.Constant.Enums;

public enum ESource {

    [Description("Trang quản lý")]
    WEB = 0,

    [Description("Máy bán hàng")]
    POS = 1,

    [Description("Trang bán hàng")]
    SALE = 2,

    [Description("Đồng bộ đơn hàng")]
    SYNC = 3
}
