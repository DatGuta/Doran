using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EWarehouseImportType {

    [Description("Nhập từ nhà cung cấp")]
    Supplier = 0,

    [Description("Nhập khác")]
    Other = 1,
}
