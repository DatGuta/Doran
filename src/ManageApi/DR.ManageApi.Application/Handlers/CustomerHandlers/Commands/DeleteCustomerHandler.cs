using DR.Common.Exceptions;
using DR.Contexts.AuditContexts;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.CustomerHandlers.Commands;

public class DeleteCustomerCommand : SingleRequest {
}

internal class DeleteCustomerHandler(IServiceProvider serviceProvider) : BaseHandler<DeleteCustomerCommand>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken) {
        var entity = await db.Customers.FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete, cancellationToken);
        ManagedException.ThrowIfNull(entity, Messages.Customer.Delete.Customer_NotFound);

        entity.IsDelete = true;

        await db.SaveChangesAsync(cancellationToken);
        await this.mediator.Publish(new CustomerDAuditContext(request.MerchantId, request.UserId, entity), cancellationToken);
    }
}
