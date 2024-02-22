namespace DR.ManageApi.Application.Handlers.CustomerGroupHandlers.Queries;

public class AllCustomerGroupQuery : Request<ListCustomerGroupData> { }

internal class AllCustomerGroupHandler(IServiceProvider serviceProvider) : BaseHandler<AllCustomerGroupQuery, ListCustomerGroupData>(serviceProvider) {

    public override async Task<ListCustomerGroupData> Handle(AllCustomerGroupQuery request, CancellationToken cancellationToken) {
        var items = await db.CustomerGroups.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
            .Select(o => CustomerGroupDto.FromEntity(o)).ToListAsync(cancellationToken);

        return new ListCustomerGroupData {
            Count = items.Count,
            Items = items
        };
    }
}
