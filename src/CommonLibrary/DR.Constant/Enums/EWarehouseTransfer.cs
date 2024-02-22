using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EWarehouseTransfer {

    [Description("Phiếu tạm")]
    Draft = 0,

    [Description("Đã chuyển")]
    Sent = 1,

    [Description("Hoàn thành")]
    Completed = 5,

    [Description("Đã huỷ")]
    Void = 9,
}
