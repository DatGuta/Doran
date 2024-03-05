namespace DR.Helper;

public static class GuidHelper {

    public static Guid New() {
        return Guid.NewGuid();
    }

    public static Guid New(Guid? existedId) {
        return existedId ?? New();
    }

    public static bool IfNullOrEmpty(this Guid? guid) {
        return guid == null || guid == Guid.Empty;
    }
}
