using DR.Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace DR.Telegram;

internal class UpdateHandler : IUpdateHandler {

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) {
        await Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        var callbackQuery = update.CallbackQuery;
        if (callbackQuery == null) return;

        var message = callbackQuery.Message;
        if (message == null) return;

        var data = Callback.Parse(callbackQuery.Data);
        if (data == null) return;

        switch ((EWarningMessage)data.Type) {
            case EWarningMessage.Stock:
                await this.HandleReplyStockWarningMessage(botClient, message, callbackQuery.From, data, cancellationToken);
                break;
        }
    }

    private async Task HandleReplyStockWarningMessage(ITelegramBotClient botClient,
        Message message, User fromUser, Callback callback, CancellationToken cancellationToken) {
        long chatId = message.Chat.Id;
        int messageId = message.MessageId;
        string from = fromUser.ToString();

        if (string.IsNullOrWhiteSpace(callback.Data)) return;

        var dataSegments = callback.Data.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (dataSegments.Length == 0) return;

        bool isSkip = dataSegments[0] == "0";
        bool isDelete = dataSegments[0] == "1";
        bool isRecalculate = dataSegments[0] == "2";

        string itemId = dataSegments.Length > 1 ? dataSegments[1] : "";

        string text = message.Text ?? string.Empty;
        string now = DateTimeOffset.Now.ToString("dd/MM/yyyy HH:mm:ss.fff zzz");
        if ((isDelete || isRecalculate) && !string.IsNullOrWhiteSpace(itemId)) {
            if (isDelete) {
                text += $"{Environment.NewLine} Deleted by {from} at {now}";
            } else {
                text += $"{Environment.NewLine} Recalculated by {from} at {now}";
            }

            ProcessWarningStock(itemId, isDelete, cancellationToken);
        } else if (isSkip) {
            text += $"{Environment.NewLine} Skiped by {from} at {now}";
        }

        await botClient.EditMessageTextAsync(
            chatId,
            messageId,
            text,
            entities: message.Entities,
            cancellationToken: cancellationToken);
    }

    private static void ProcessWarningStock(string itemId, bool isDelete, CancellationToken cancellationToken) {
        using var scope = Container.Instance.CreateScope();
        IBus bus = scope.ServiceProvider.GetRequiredService<IBusControl>();
        //        await bus.Publish(new ProcessWarningStockContext(itemId, isDelete), cancellationToken);
    }
}
