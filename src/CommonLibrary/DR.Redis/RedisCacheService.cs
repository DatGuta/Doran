using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DR.Redis {

    public class RedisConfiguration {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    public interface IRedisCacheService {

        Task<T?> GetAsync<T>(string key);

        Task SetAsync(string key, object? data, TimeSpan? ttl = null);

        Task RemoveAsync(string key);

        bool TryGetValue<T>(string key, out T? result);
    }

    public class RedisCacheService : IRedisCacheService {
        private readonly IDatabase redisCache;

        public RedisCacheService(IServiceProvider serviceProvider) {
            this.redisCache = serviceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key) {
            var json = await redisCache.StringGetAsync(key);
            if (json.HasValue) {
                return JsonConvert.DeserializeObject<T>(json.ToString());
            }

            return default;
        }

        public async Task RemoveAsync(string key) {
            await this.redisCache.KeyDeleteAsync(key);
        }

        public async Task SetAsync(string key, object? data, TimeSpan? ttl = null) {
            var json = JsonConvert.SerializeObject(data);
            await this.redisCache.StringSetAsync(key, json);
            if (ttl.HasValue && ttl.Value > TimeSpan.Zero)
                await this.redisCache.KeyExpireAsync(key, ttl);
        }

        public bool TryGetValue<T>(string key, out T? result) {
            var existed = this.redisCache.KeyExistsAsync(key).Result;
            if (existed) {
                result = GetAsync<T>(key).Result;
                return true;
            }
            result = default;
            return false;
        }
    }
}
