using DR.Contexts.NormalContexts;
using MassTransit;
using Quartz;

namespace DR.ManageApi.Application.Jobs;

public class RetryJob(IServiceProvider serviceProvider) : IJob {
    private readonly FmsContext db = serviceProvider.GetRequiredService<FmsContext>();
    private readonly IBusControl bus = serviceProvider.GetRequiredService<IBusControl>();

    public async Task Execute(IJobExecutionContext context) {
        await this.RetryOnHand();
        await this.RetryCustomerDebt();
    }

    private async Task RetryOnHand() {
        var contexts = await this.db.ProductTrackings.Where(o => !o.IsUpdateOnHand)
            .GroupBy(o => new UpdateProductTrackingContext(o.MerchantId, o.WarehouseId, o.ProductId))
            .Select(o => o.Key).ToListAsync();

        foreach (var context in contexts) {
            await this.bus.Publish(context);
        }
    }

    private async Task RetryCustomerDebt() {
        var contexts = await this.db.CustomerTrackings.Where(o => !o.IsUpdate)
            .GroupBy(o => new UpdateCustomerTrackingContext(o.MerchantId, o.CustomerId))
            .Select(o => o.Key).ToListAsync();

        foreach (var context in contexts) {
            await this.bus.Publish(context);
        }
    }
}
