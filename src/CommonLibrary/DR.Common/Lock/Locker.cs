namespace DR.Common.Lock;

public static class Locker {

    public static async Task LockByKey(string key, int expirySec = 60) {
        using var locker = await AsyncLocker.LockAsync(key, expirySec);
    }

    public static async Task LockByKey(string key, Func<Task> action, int expirySec = 60) {
        using var locker = await AsyncLocker.LockAsync(key, expirySec);
        await action.Invoke();
    }

    public static async Task<T> LockByKey<T>(string key, Func<Task<T>> action, int expirySec = 60) {
        using var locker = await AsyncLocker.LockAsync(key, expirySec);
        return await action.Invoke();
    }
}
