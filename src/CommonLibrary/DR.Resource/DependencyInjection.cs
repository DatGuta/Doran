using Microsoft.Extensions.DependencyInjection;

namespace DR.Resource;

public static class DependencyInjection {

    public static IServiceCollection AddResources(this IServiceCollection services) {
        services.AddSingleton<UnitRes>();
        return services;
    }
}
