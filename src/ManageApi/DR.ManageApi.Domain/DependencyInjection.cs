using DR.ManageApi.Domain.Services.Implements;
using DR.ManageApi.Domain.Services.Interfaces;

namespace DR.ManageApi.Domain;

public static class DependencyInjection {

    public static IServiceCollection AddServices(this IServiceCollection services) {
        services.AddScoped<IAutoGenerationService, AutoGenerationService>();
        services.AddScoped<ICultureInfoService, CultureInfoService>();
        services.AddScoped<ICustomerTrackingService, CustomerTrackingService>();
        services.AddScoped<IGlobalSettingService, GlobalSettingService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IMerchantItemDefaultService, MerchantItemDefaultService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductTrackingService, ProductTrackingService>();
        services.AddScoped<ISettingService, SettingService>();
        return services;
    }
}
