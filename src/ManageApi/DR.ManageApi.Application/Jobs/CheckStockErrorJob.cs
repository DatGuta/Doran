using DR.Common.Extensions;
using DR.Configuration;
using DR.Constant.Enums;
using DR.Database.Models;
using DR.Telegram;
using Markdown;
using Microsoft.Extensions.Configuration;
using Quartz;

namespace FMS.ManageApi.Application.Jobs;

[DisallowConcurrentExecution]
public class CheckStockErrorJob(IServiceProvider serviceProvider) : IJob {
    private readonly IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    private readonly FmsContext db = serviceProvider.GetRequiredService<FmsContext>();
    private readonly ITelegramService telegramService = serviceProvider.GetRequiredService<ITelegramService>();

    public async Task Execute(IJobExecutionContext context) {
        var stockErrorConfig = configuration.GetSection("TelegramBot:StockError").Get<TelegramBotConfig>();
        if (stockErrorConfig == null || stockErrorConfig.ChatIds.Count == 0)
            return;

        var wrongStockIds = await this.GetWrongStockIds();
        var jumpStockIds = await this.GetJumpStockIds();

        var messages = await this.GetWarningMessages(wrongStockIds, jumpStockIds);
        await this.SendWarningMessages(messages, stockErrorConfig.ChatIds);
    }

    private async Task<List<string>> GetWrongStockIds() {
        return await this.db.ProductTrackings.AsNoTracking()
            .Where(o => !o.IsDelete && (o.OnHandBefore + o.Quantity != o.OnHandAfter)
                && o.Warehouse!.Type == EWarehouse.Normal)
            .Select(o => o.Id).ToListAsync();
    }

    private async Task<List<string>> GetJumpStockIds() {
        var query = $@"
WITH tracking AS (
	SELECT ""Id"", ""OnHandBefore"",
		LAG(""OnHandAfter"")
		OVER (
			PARTITION BY ""MerchantId"", ""WarehouseId"", ""ProductId""
			ORDER BY ""Date""
		) AS ""LastOnHandAfter""
	FROM public.""ProductTracking""
	WHERE ""IsDelete"" = false
	ORDER BY ""ProductId"", ""Date"" DESC
)
SELECT ""Id""
FROM tracking
WHERE ""OnHandBefore"" <> ""LastOnHandAfter""";

        return await this.db.Database.SqlQueryRaw<string>(query).ToListAsync();
    }

    private async Task<List<WarningMessage>> GetWarningMessages(List<string> wrongStockIds, List<string> jumpStockIds) {
        var errorStockIds = wrongStockIds.Concat(jumpStockIds).Distinct().ToList();
        var items = await this.db.ProductTrackings.AsNoTracking()
            .Where(o => errorStockIds.Contains(o.Id)).ToListAsync();

        var merchantIds = items.Select(o => o.MerchantId).Distinct().ToList();
        var merchants = await this.db.Merchants.AsNoTracking()
            .Where(o => merchantIds.Contains(o.Id))
            .ToDictionaryAsync(k => k.Id, v => new { v.Code, v.Name });

        var warehouseIds = items.Select(o => o.WarehouseId).Distinct().ToList();
        var warehouses = await this.db.Warehouses.AsNoTracking()
            .Where(o => warehouseIds.Contains(o.Id))
            .ToDictionaryAsync(k => k.Id, v => new { v.Code, v.Name });

        var productIds = items.Select(o => o.ProductId).Distinct().ToList();
        var products = await this.db.Products.AsNoTracking()
            .Where(o => productIds.Contains(o.Id))
            .ToDictionaryAsync(k => k.Id, v => new { v.Code, v.Name });

        var messages = new List<WarningMessage>();
        foreach (var item in items) {
            var message = new WarningMessage();

            var merchant = merchants.GetValueOrDefault(item.MerchantId);
            var warehouse = warehouses.GetValueOrDefault(item.WarehouseId);
            var product = products.GetValueOrDefault(item.ProductId);

            var reasons = new List<string>();
            if (wrongStockIds.Contains(item.Id)) {
                reasons.Add("miscalculation");
            }
            if (jumpStockIds.Contains(item.Id)) {
                reasons.Add("not consistent");
            }

            message.Markdown.AppendList(
                new MarkdownTextListItem($"{nameof(item.Id)}: `{item.Id}`"),
                new MarkdownTextListItem($"{nameof(Merchant)}: `{item.MerchantId}` ({merchant?.Code.MarkdownStandardized()}) {merchant?.Name.MarkdownStandardized()}"),
                new MarkdownTextListItem($"{nameof(Warehouse)}: `{item.WarehouseId}` ({warehouse?.Code.MarkdownStandardized()}) {warehouse?.Name.MarkdownStandardized()}"),
                new MarkdownTextListItem($"{nameof(Product)}: `{item.ProductId}` ({product?.Code.MarkdownStandardized()}) {product?.Name.MarkdownStandardized()}"),
                new MarkdownTextListItem($"{nameof(item.DocumentType)}: {item.DocumentType.Description()}"),
                new MarkdownTextListItem($"{nameof(item.DocumentId)}: `{item.DocumentId}`"),
                new MarkdownTextListItem($"{nameof(item.DocumentCode)}: `{item.DocumentCode}`"),
                new MarkdownTextListItem($"{nameof(item.TableName)}: `{item.TableName}`"),
                new MarkdownTextListItem($"{nameof(item.ItemId)}: `{item.ItemId}`"),
                new MarkdownTextListItem($"{nameof(item.OnHandBefore)}: {item.OnHandBefore}"),
                new MarkdownTextListItem($"{nameof(item.Quantity)}: {item.Quantity}"),
                new MarkdownTextListItem($"{nameof(item.OnHandAfter)}: {item.OnHandAfter}"),
                new MarkdownTextListItem($"{nameof(item.Date)}: `{item.Date.ToUnixTimeMilliseconds()}` | {item.Date.ToLocalTime():dd/MM/yyyy HH:mm:ss.fff}"),
                new MarkdownTextListItem($"{nameof(item.IsUpdateOnHand)}: {item.IsUpdateOnHand}"),
                new MarkdownTextListItem($"{nameof(item.IsDelete)}: {item.IsDelete}"),
                new MarkdownTextListItem($"Reason: {string.Join(", ", reasons)}")
            );
            message.ReplyActions.Add("Skip", new Callback((int)EWarningMessage.Stock, $"0"));
            message.ReplyActions.Add("Delete", new Callback((int)EWarningMessage.Stock, $"1:{item.Id}"));
            message.ReplyActions.Add("Recalculate", new Callback((int)EWarningMessage.Stock, $"2:{item.Id}"));

            messages.Add(message);
        }
        return messages;
    }

    private async Task SendWarningMessages(List<WarningMessage> messages, List<long> chatIds) {
        foreach (var message in messages) {
            foreach (var chatId in chatIds) {
                await this.telegramService.SendMessage(ETelegramBot.StockError, chatId,
                    message.Markdown.ToString(), message.ReplyActions);
            }
        }
    }

    private sealed class WarningMessage {
        public IMarkdownDocument Markdown { get; } = new MarkdownDocument();
        public Dictionary<string, Callback> ReplyActions { get; } = new();
    }
}
