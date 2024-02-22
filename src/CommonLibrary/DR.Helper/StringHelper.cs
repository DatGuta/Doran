using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DR.Helper;

public static class StringHelper {

    [return: NotNullIfNotNull(nameof(text))]
    public static string? UnsignedUnicode(string? text) {
        if (string.IsNullOrWhiteSpace(text)) return text;
        var chars = text.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
        return new string(chars).Normalize(NormalizationForm.FormC).Trim().ToLower().Replace('đ', 'd');
    }

    public static string UnsignedUnicode(string? text, string defaultValue) {
        return UnsignedUnicode(text) ?? defaultValue;
    }

    public static string GetLast(string text, int length = 4) {
        if (string.IsNullOrWhiteSpace(text) || length >= text.Length)
            return text;
        return text[^length..];
    }

    [return: NotNullIfNotNull(nameof(text))]
    public static string? ReplaceSpace(string? text, bool isUnsignedUnicode = false) {
        if (text == null) return text;
        text = Regex.Replace(text.Trim(), @"\s+", " ");

        if (isUnsignedUnicode) {
            text = UnsignedUnicode(text);
        }
        return text;
    }

    [return: NotNullIfNotNull(nameof(text))]
    public static string? ReplaceSpaceToHyphen(string? text) {
        if (text == null) return text;
        text = text.Trim();

        return text.Replace(" ", "-");
    }
}
