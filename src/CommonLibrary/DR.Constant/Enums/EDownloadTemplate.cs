using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EDownloadTemplate {

    [Description(".xlsx")]
    XLSX = 0,

    [Description(".xls")]
    XLS = 1,

    [Description(".csv")]
    CSV = 2
}
