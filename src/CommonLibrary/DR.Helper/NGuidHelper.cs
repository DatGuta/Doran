namespace DR.Helper;

public static class NGuidHelper {

    public static string New() {
        return Guid.NewGuid().ToString("N");
    }

    public static string New(string? existedId) {
        return string.IsNullOrWhiteSpace(existedId) ? New() : existedId;
    }
}
