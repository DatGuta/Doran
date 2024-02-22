namespace DR.Telegram;
public interface ITelegramService {

    Task SendMessage(ETelegramBot bot, long chatId, string text, Dictionary<string, Callback>? callback, CancellationToken cancellationToken = default);

    Task SendMessage(ETelegramBot bot, long chatId, string text, List<Dictionary<string, Callback>>? callback, CancellationToken cancellationToken = default);
}
