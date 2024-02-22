namespace DR.ManageApi.Application.Handlers.CustomerGroupHandlers.Queries;

public class GetCustomerGroupQuery : SingleRequest<CustomerGroupDto?> { }

internal class GetCustomerGroupHandler(IServiceProvider serviceProvider) : BaseHandler<GetCustomerGroupQuery, CustomerGroupDto?>(serviceProvider) {

    public override async Task<CustomerGroupDto?> Handle(GetCustomerGroupQuery request, CancellationToken cancellationToken) {
        return await db.CustomerGroups.AsNoTracking()
             .Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete)
             .Select(o => CustomerGroupDto.FromEntity(o))
             .FirstOrDefaultAsync(cancellationToken);
    }
}
