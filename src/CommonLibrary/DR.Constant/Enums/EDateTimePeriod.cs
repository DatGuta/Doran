using System.ComponentModel;

namespace DR.Constant.Enums;

public enum EDateTimePeriod {

    [Description("Ngày")]
    Day = 1,

    [Description("Tuần")]
    Week = 2,

    [Description("Tháng")]
    Month = 3,

    [Description("Năm")]
    Year = 4,
}
