using DR.Contexts.AuditContexts;
using DR.Contexts.NormalContexts;
using MassTransit;

namespace FMS.ManageApi.Application.Handlers.CustomerGroupHandlers.Commands;

public class SaveCusByCusGrpCommand : Request {
    public string CustomerGroupId { get; set; } = string.Empty;
    public List<string> CustomerIds { get; set; } = [];
}

internal class SaveCusByCusGrpHandler(IServiceProvider serviceProvider) : BaseHandler<SaveCusByCusGrpCommand>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
    private readonly IBus bus = serviceProvider.GetRequiredService<IBus>();

    public override async Task Handle(SaveCusByCusGrpCommand request, CancellationToken cancellationToken) {
        var merchantId = request.MerchantId;
        var userId = request.UserId;
        var customerIds = request.CustomerIds ?? [];

        if (string.IsNullOrWhiteSpace(request.CustomerGroupId)) return;

        var cusGrp = await db.CustomerGroups.AsNoTracking()
            .FirstOrDefaultAsync(o => o.MerchantId == merchantId && o.Id == request.CustomerGroupId && !o.IsDelete, cancellationToken);
        if (cusGrp == null) return;

        var customers = await db.Customers.Where(o => o.MerchantId == merchantId && !o.IsDelete
            && (customerIds.Contains(o.Id) || o.CustomerGroupId == cusGrp.Id)).ToListAsync(cancellationToken);

        var set = new HashSet<string>() { cusGrp.Id };
        var auditItems = new List<CusOnCusGrpAuditItem>();
        foreach (var customer in customers) {
            if (!string.IsNullOrWhiteSpace(customer.CustomerGroupId))
                set.Add(customer.CustomerGroupId);

            var auditItem = new CusOnCusGrpAuditItem {
                CustomerId = customer.Id,
                OldCusGrpId = customer.CustomerGroupId,
            };

            customer.CustomerGroupId = customerIds.Contains(customer.Id) ? cusGrp.Id : null;

            auditItem.NewCusGrpId = customer.CustomerGroupId;
            auditItems.Add(auditItem);
        }

        await db.SaveChangesAsync(cancellationToken);

        await this.mediator.Publish(new CusOnCusGrpUAuditContext(merchantId, userId) {
            CustomerGroupCode = cusGrp.Code,
            Items = auditItems,
        }, cancellationToken);
        await this.bus.Publish(new UpdateNoOfCusByCusGrpContext(merchantId, set), cancellationToken);
    }
}
