using DR.Contexts.NormalContexts;

namespace DR.ManageApi.Application.Consumers.NormalConsumers;

public class UpdateNoOfCusByCusGrpConsumer(IServiceProvider serviceProvider)
    : BaseRabbitMqConsumer<UpdateNoOfCusByCusGrpContext>(serviceProvider) {

    public override async Task Handle(UpdateNoOfCusByCusGrpContext context, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(context.MerchantId)
                || context.CustomerGroupIds.Count == 0) {
            return;
        }
        var cusGrps = await db.CustomerGroups.Where(o => o.MerchantId == context.MerchantId && !o.IsDelete && context.CustomerGroupIds.Contains(o.Id)).ToListAsync(cancellationToken);
        if (cusGrps.Count == 0) return;
        var noCusByGrps = await db.Customers.AsNoTracking()
            .Where(o => o.MerchantId == context.MerchantId && !o.IsDelete
                && !string.IsNullOrEmpty(o.CustomerGroupId) && context.CustomerGroupIds.Contains(o.CustomerGroupId))
            .GroupBy(o => o.CustomerGroupId!)
            .ToDictionaryAsync(k => k.Key, v => v.Select(o => o.Id).Count(), cancellationToken);

        foreach (var cusGrp in cusGrps) {
            cusGrp.NumberOfCustomers = noCusByGrps.GetValueOrDefault(cusGrp.Id);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
