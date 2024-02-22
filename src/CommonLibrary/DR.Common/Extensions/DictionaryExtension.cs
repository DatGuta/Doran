namespace DR.Common.Extensions;

public static class DictionaryExtension {

    public static T GetValue<T>(this Dictionary<string, T> dict, string? key, T defaultValue)
        => !string.IsNullOrWhiteSpace(key) ? dict.GetValueOrDefault(key, defaultValue) : defaultValue;

    public static T? GetValue<T>(this Dictionary<string, T> dict, string? key)
        => !string.IsNullOrWhiteSpace(key) ? dict.GetValueOrDefault(key) : default;

    public static string GetValueString(this Dictionary<string, string> dict, string? key)
        => !string.IsNullOrWhiteSpace(key) ? dict.GetValueOrDefault(key, string.Empty) : string.Empty;
}
