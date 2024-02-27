using DR.Handlers;
namespace DR.ManageApi.Application;

public static class DependencyInjection {

    public static IServiceCollection AddHandlers(this IServiceCollection services) {
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<BaseHandler>());
        return services;
    }
}
