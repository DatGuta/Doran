using DR.Constant.Enums;

namespace DR.Database.ExtendModels;

public class AutoGenerate {

    [Description("Tiền tố")]
    public string Prefix { get; set; } = null!;

    [Description("Số bắt đầu")]
    public int StartNumber { get; set; }

    [Description("Số tăng")]
    public int IncrNumber { get; set; }

    [Description("Tăng ngẩu nhiên")]
    public bool IsRandomIncrNumber { get; set; }

    [Description("Đặt lại theo chu kỳ")]
    public EDateTimePeriod? ResetBy { get; set; }
}
