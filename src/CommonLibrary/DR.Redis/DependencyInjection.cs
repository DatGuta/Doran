using DR.Configuration;
using DR.Redis.Implements;
using DR.Redis.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DR.Redis;

public static class DependencyInjection {

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration) {
        var config = configuration.GetSection("Redis").Get<RedisConfig>();
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect($"{config?.Host}:{config?.Port},password={config?.Password}"));
        services.AddScoped<IRedisService, RedisService>();
        return services;
    }
}
