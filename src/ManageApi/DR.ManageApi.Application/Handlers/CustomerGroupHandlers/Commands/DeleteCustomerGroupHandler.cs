using DR.Common.Exceptions;
using DR.Contexts.AuditContexts;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.CustomerGroupHandlers.Commands;

public class DeleteCustomerGroupCommand : SingleRequest { }

internal class DeleteCustomerGroupHandler(IServiceProvider serviceProvider) : BaseHandler<DeleteCustomerGroupCommand>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task Handle(DeleteCustomerGroupCommand request, CancellationToken cancellationToken) {
        var entity = await db.CustomerGroups.FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete, cancellationToken);
        ManagedException.ThrowIfNull(entity, Messages.CustomerGroup.Delete.Customer_NotFound);

        var existedCustomers = await db.Customers.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete && o.CustomerGroupId == entity.Id)
            .AnyAsync(cancellationToken);
        ManagedException.ThrowIf(existedCustomers, Messages.CustomerGroup.Delete.Customer_DeleteFail);

        entity.IsDelete = true;
        entity.ModifiedDate = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new CusGrpDAuditContext(request.MerchantId, request.UserId, entity), cancellationToken);
    }
}
