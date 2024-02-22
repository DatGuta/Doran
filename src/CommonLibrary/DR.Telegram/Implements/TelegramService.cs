using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DR.Telegram.Implements;

public class TelegramService : ITelegramService {
    private readonly IEnumerable<ITelegramBotClient> botClients;

    public TelegramService(IServiceProvider serviceProvider) {
        this.botClients = serviceProvider.GetServices<ITelegramBotClient>();
    }

    public async Task SendMessage(ETelegramBot bot, long chatId, string text, Dictionary<string, Callback>? callback, CancellationToken cancellationToken = default) {
        List<Dictionary<string, Callback>>? list = null;
        if (callback != null) {
            list = [callback];
        }
        await this.SendMessage(bot, chatId, text, list, cancellationToken);
    }

    public async Task SendMessage(ETelegramBot bot, long chatId, string text, List<Dictionary<string, Callback>>? callback, CancellationToken cancellationToken = default) {
        IReplyMarkup? replyMarkup = null;
        if (callback != null && callback.Count > 0) {
            var inlineKeyboardButtons = new List<List<InlineKeyboardButton>>();

            foreach (var row in callback) {
                var rowInlineKeyboardButtons = new List<InlineKeyboardButton>();

                foreach (var button in row) {
                    var inlineButton = InlineKeyboardButton.WithCallbackData(button.Key, button.Value.ToString());
                    rowInlineKeyboardButtons.Add(inlineButton);
                }

                inlineKeyboardButtons.Add(rowInlineKeyboardButtons);
            }

            replyMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);
        }

        var botClient = this.GetBotClient(bot);
        await botClient.SendTextMessageAsync(
            chatId,
            text,
            parseMode: ParseMode.Markdown,
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken);
    }

    private ITelegramBotClient GetBotClient(ETelegramBot bot) {
        return this.botClients?.FirstOrDefault(o => o.BotId == (long)bot)
            ?? throw new NotSupportedException($"Telegram bot: {bot}");
    }
}
