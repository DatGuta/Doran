using DR.Configuration;
using DR.ManageApi.Application.Consumers;
using DR.ManageApi.Application.Handlers;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace DR.ManageApi.Application;

public static class DependencyInjection {

    public static IServiceCollection AddHandlers(this IServiceCollection services) {
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<BaseHandler>());
        return services;
    }

    public static IServiceCollection AddJobs(this IServiceCollection services) {


        return services;
    }

    public static IServiceCollection AddConsumers(this IServiceCollection services, IConfiguration configuration) {
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<BaseConsumer>());

        var rabbitMqConfig = configuration.GetSection("RabbitMq").Get<RabbitMqConfig>();
        if (rabbitMqConfig != null) {
            services.AddMassTransit(o => {
                o.AddConsumers(typeof(BaseRabbitMqConsumer).Assembly);

                o.UsingRabbitMq((context, cfg) => {
                    cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, rabbitMqConfig.VirtualHost, auth => {
                        auth.Username(rabbitMqConfig.Username);
                        auth.Password(rabbitMqConfig.Password);
                    });
                    cfg.ConfigureEndpoints(context);
                    cfg.ReceiveEndpoint(e => e.ConfigureConsumers(context));
                });
            });
        }

        return services;
    }
}
