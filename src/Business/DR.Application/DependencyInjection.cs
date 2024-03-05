using DR.Application.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace DR.Application;

public static class DependencyInjection {

    public static IServiceCollection AddMediatR(this IServiceCollection services, IConfiguration configuration) {
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<BaseMediatR>());

        // Add RabbitMq

        //var rabbitMqConfig = configuration.GetSection("RabbitMq").Get<RabbitMqConfig>();
        //if (rabbitMqConfig != null) {
        //    services.AddMassTransit(o => {
        //        if (rabbitMqConfig.RunConsumer) {
        //            o.AddConsumers(typeof(BaseRabbitMqConsumer).Assembly);
        //        }

        //        o.UsingRabbitMq((context, cfg) => {
        //            cfg.Host(rabbitMqConfig.Host, rabbitMqConfig.Port, rabbitMqConfig.VirtualHost, auth => {
        //                auth.Username(rabbitMqConfig.Username);
        //                auth.Password(rabbitMqConfig.Password);
        //            });
        //            cfg.ConfigureEndpoints(context);
        //            cfg.ReceiveEndpoint(e => e.ConfigureConsumers(context));
        //        });
        //    });
        //}

        return services;
    }

    public static IServiceCollection AddMiddlewares(this IServiceCollection services) {
        //  services.AddTransient<SessionMiddleware>();
        services.AddTransient<ExceptionMiddleware>();
        return services;
    }

    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app) {
        // app.UseMiddleware<SessionMiddleware>(); 
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}
