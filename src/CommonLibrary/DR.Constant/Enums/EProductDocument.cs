using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EProductDocumentType {

    [Description("Nhập kho")]
    Import = 1,

    [Description("Xuất kho")]
    Export = 2,

    [Description("Chuyển kho")]
    Transfer = 3,

    [Description("Trả hàng")]
    Refund = 4,

    [Description("Kiểm kho")]
    Adjustment = 5,

    [Description("Xuất khác")]
    ExportOther = 6,

    [Description("Nhập khác")]
    ImportOther = 7,
}
