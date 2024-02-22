using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EWarehouseImport {

    [Description("Phiếu tạm")]
    Draft = 0,

    [Description("Hoàn thành")]
    Completed = 5,

    [Description("Đã huỷ")]
    Void = 9,
}
