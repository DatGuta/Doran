using System.Globalization;

namespace DR.Database.ExtendModels;

public class NumberFormat {

    [Description("Dấu phân cách phần nghìn")]
    public string ThousandsSeparator { get; set; } = ",";

    [Description("Dấu phân cách phần thập phân")]
    public string DecimalSeparator { get; set; } = ".";

    [Description("Số chữ số thập phân cho số lượng")]
    public int NumberDecimalDigitsForQuantity { get; set; } = 1;

    [Description("Số chữ số thập phân cho số tiền")]
    public int NumberDecimalDigitsForCurrency { get; set; } = 0;

    public CultureInfo GetCultureInfo() {
        var cultureInfo = new CultureInfo("en-US");
        cultureInfo.NumberFormat.NumberDecimalDigits = NumberDecimalDigitsForQuantity;
        cultureInfo.NumberFormat.NumberGroupSeparator = ThousandsSeparator;
        cultureInfo.NumberFormat.NumberDecimalSeparator = DecimalSeparator;
        cultureInfo.NumberFormat.CurrencySymbol = "";
        cultureInfo.NumberFormat.CurrencyDecimalDigits = NumberDecimalDigitsForCurrency;
        cultureInfo.NumberFormat.CurrencyGroupSeparator = ThousandsSeparator;
        cultureInfo.NumberFormat.CurrencyDecimalSeparator = DecimalSeparator;
        return cultureInfo;
    }
}
