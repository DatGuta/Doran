using System.Globalization;

namespace DR.Export.Extentions {

    public class NumberFormatter {
        public static NumberFormatInfo NumberFormatInfo { get; private set; }

        public NumberFormatter(NumberFormatInfo info) {
            NumberFormatInfo = info ?? CultureInfo.CurrentCulture.NumberFormat;
        }

        public string Format(decimal value, string format = "N") {
            return value.ToString(format, NumberFormatInfo);
        }
    }
}
