using DR.Resource;

namespace DR.ManageApi.Application.Handlers.CustomerHandlers.Queries;

public class GetCustomerQuery : SingleRequest<CustomerDto?> { }

internal class GetCustomerHandler(IServiceProvider serviceProvider) : BaseHandler<GetCustomerQuery, CustomerDto?>(serviceProvider) {
    private readonly UnitResource unitResource = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<CustomerDto?> Handle(GetCustomerQuery request, CancellationToken cancellationToken) {
        var customer = await db.Customers.Include(o => o.CustomerGroup).AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete)
            .Select(o => CustomerDto.FromEntity(o, unitResource))
            .FirstOrDefaultAsync(cancellationToken);

        return customer;
    }
}
