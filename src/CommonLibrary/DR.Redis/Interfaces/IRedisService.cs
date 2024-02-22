namespace DR.Redis;

public interface IRedisService {

    Task<T?> GetAsync<T>(string key);

    Task SetAsync(string key, object? data, TimeSpan? ttl = null);

    Task RemoveAsync(string key);

    bool TryGetValue<T>(string key, out T? result);
}
