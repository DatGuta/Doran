using Crypto.AES;

namespace DR.Helper;

public static class PhoneHelper {
    private const string Key = "tuanvudev";

    public static string? Encrypt(string? text) {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        try {
            return AES.EncryptString(Key, text);
        } catch {
            return text;
        }
    }

    public static string? Decrypt(string? text) {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        try {
            return AES.DecryptString(Key, text);
        } catch {
            return text;
        }
    }
}
