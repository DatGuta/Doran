using DR.Common.Exceptions;
using DR.Constant.Enums;
using DR.Contexts.AuditContexts;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.CustomerDebtHandlers.Commands;

public class SaveInitDebtCommand : Request {
    public string CustomerId { get; set; } = string.Empty;
    public decimal Debt { get; set; }
}

internal class SaveInitDebtHandler(IServiceProvider serviceProvider) : BaseHandler<SaveInitDebtCommand>(serviceProvider) {
    private readonly ICustomerTrackingService customerTrackingService = serviceProvider.GetRequiredService<ICustomerTrackingService>();
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task Handle(SaveInitDebtCommand request, CancellationToken cancellationToken) {
        ManagedException.ThrowIf(request.Debt < 0, Messages.CustomerDebt.Debt_Invalid);

        var customer = await db.Customers.AsNoTracking()
            .FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.CustomerId, cancellationToken);
        ManagedException.ThrowIfNull(customer, Messages.CustomerDebt.Customer_NotFound);

        var tracking = await db.CustomerTrackings.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.CustomerId == customer.Id && o.DocumentType == ECustomerDocType.Init)
            .FirstOrDefaultAsync(cancellationToken);

        await customerTrackingService.ProcessInit(request.MerchantId, request.CustomerId, request.Debt, cancellationToken);

        if (tracking == null) {
            await mediator.Publish(new CustomerInitDebtCAuditContext(request.MerchantId, request.UserId, customer, request.Debt), cancellationToken);
        } else {
            await mediator.Publish(new CustomerInitDebtUAuditContext(request.MerchantId, request.UserId, customer, tracking.Debt, request.Debt), cancellationToken);
        }
    }
}
