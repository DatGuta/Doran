using System.Globalization;

namespace DR.Helper;

public static class NumberHelper {

    public static string N(decimal? value, CultureInfo? cultureInfo = null)
        => Format(value, "N", cultureInfo);

    public static string C(decimal? value, CultureInfo? cultureInfo = null)
        => Format(value, "C", cultureInfo);

    private static string Format(decimal? value, string format, CultureInfo? cultureInfo = null)
        => value?.ToString(format, cultureInfo ?? CultureInfo.CurrentCulture) ?? string.Empty;
}
