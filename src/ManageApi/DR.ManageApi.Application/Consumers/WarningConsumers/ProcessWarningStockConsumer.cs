using DR.Contexts.NormalContexts;
using DR.Contexts.WarningContexts;

namespace DR.ManageApi.Application.Consumers.WarningConsumers;

public class ProcessWarningStockConsumer(IServiceProvider serviceProvider)
    : BaseRabbitMqConsumer<ProcessWarningStockContext>(serviceProvider) {

    public override async Task Handle(ProcessWarningStockContext context, CancellationToken cancellationToken) {
        var item = await this.db.ProductTrackings.FirstOrDefaultAsync(o => o.Id == context.Id, cancellationToken);
        if (item == null) return;

        item.IsUpdateOnHand = false;
        if (context.IsDelete) {
            item.IsDelete = true;
        }

        await this.db.SaveChangesAsync(cancellationToken);

        var ctx = new UpdateProductTrackingContext(item.MerchantId, item.WarehouseId, item.ProductId);
        await this.bus.Publish(ctx, cancellationToken);
    }
}
