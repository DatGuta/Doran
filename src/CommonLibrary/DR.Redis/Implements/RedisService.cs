using DR.Redis.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DR.Redis.Implements;

public class RedisService : IRedisService {
    private readonly IDatabase redisCache;

    public RedisService(IServiceProvider serviceProvider) {
        redisCache = serviceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key) {
        var json = await redisCache.StringGetAsync(key);
        if (json.HasValue) {
            return JsonConvert.DeserializeObject<T>(json.ToString());
        }

        return default;
    }

    public async Task RemoveAsync(string key) {
        await redisCache.KeyDeleteAsync(key);
    }

    public async Task SetAsync(string key, object? data, TimeSpan? ttl = null) {
        var json = JsonConvert.SerializeObject(data);
        await redisCache.StringSetAsync(key, json);
        if (ttl.HasValue && ttl.Value > TimeSpan.Zero)
            await redisCache.KeyExpireAsync(key, ttl);
    }

    public bool TryGetValue<T>(string key, out T? result) {
        var existed = redisCache.KeyExistsAsync(key).Result;
        if (existed) {
            result = GetAsync<T>(key).Result;
            return true;
        }
        result = default;
        return false;
    }
}
