using DR.Configuration;
using DR.Telegram.Implements;
using DR.Telegram.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace DR.Telegram;

public static class DependencyInjection {

    public static IServiceCollection AddTelegramBot(this IServiceCollection services, IConfiguration configuration) {
        var stockErrorConfig = configuration.GetSection("TelegramBot:StockError").Get<TelegramBotConfig>();
        if (stockErrorConfig != null && stockErrorConfig.ChatIds.Count > 0) {
            var botClient = new TelegramBotClient(stockErrorConfig.AccessToken);

            if (stockErrorConfig.IsListen) {
                var options = new ReceiverOptions {
                    AllowedUpdates = [UpdateType.CallbackQuery]
                };
                botClient.StartReceiving<UpdateHandler>(options);
            }

            services.AddSingleton<ITelegramBotClient>(botClient);
        }

        services.AddScoped<ITelegramService, TelegramService>();

        return services;
    }
}
