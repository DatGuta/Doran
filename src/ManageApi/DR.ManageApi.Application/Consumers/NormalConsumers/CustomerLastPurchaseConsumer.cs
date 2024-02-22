using DR.Constant.Enums;
using DR.Contexts.NormalContexts;

namespace DR.ManageApi.Application.Consumers.NormalConsumers;

public class CustomerLastPurchaseConsumer(IServiceProvider serviceProvider)
    : BaseRabbitMqConsumer<CustomerLastPurchaseContext>(serviceProvider) {

    public override async Task Handle(CustomerLastPurchaseContext context, CancellationToken cancellationToken) {
        var customer = await this.db.Customers
                .Where(o => o.MerchantId == context.MerchantId && o.Id == context.CustomerId && !o.IsDelete)
                .FirstOrDefaultAsync(cancellationToken);

        if (customer == null) return;

        var handleStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Ticket, EOrderStatus.Export, EOrderStatus.Exported };
        var lastOrder = await this.db.Orders.AsNoTracking()
            .Where(o => o.MerchantId == context.MerchantId && o.CustomerId == context.CustomerId && handleStatus.Contains(o.Status))
            .OrderByDescending(o => o.CreatedDate)
            .Select(o => new { o.CreatedDate })
            .FirstOrDefaultAsync(cancellationToken);

        customer.LastPurchase = lastOrder?.CreatedDate.ToUnixTimeMilliseconds() ?? -1;

        await this.db.SaveChangesAsync(cancellationToken);
    }
}
