namespace DR.Common.Extensions;

public static class StringExtension {

    public static string MarkdownStandardized(this string text) {
        return text.Replace("*", "\\*").Replace("_", "\\_");
    }
}
