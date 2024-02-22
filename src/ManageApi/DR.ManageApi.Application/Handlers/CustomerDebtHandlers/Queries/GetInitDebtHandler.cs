using DR.Common.Exceptions;
using DR.Constant.Enums;
using DR.Message;

namespace FMS.ManageApi.Application.Handlers.CustomerDebtHandlers.Queries;

public class GetInitDebtQuery : SingleRequest<CustomerInitDebtDto> { }

internal class GetInitDebtHandler(IServiceProvider serviceProvider)
    : BaseHandler<GetInitDebtQuery, CustomerInitDebtDto>(serviceProvider) {

    public override async Task<CustomerInitDebtDto> Handle(GetInitDebtQuery request, CancellationToken cancellationToken) {
        var customer = await db.Customers.AsNoTracking()
            .FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.Id, cancellationToken);
        ManagedException.ThrowIfNull(customer, Messages.CustomerDebt.Customer_NotFound);

        var tracking = await db.CustomerTrackings.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.CustomerId == customer.Id && o.DocumentType == ECustomerDocType.Init)
            .FirstOrDefaultAsync(cancellationToken);

        return new CustomerInitDebtDto {
            Id = customer.Id,
            Code = customer.Code,
            Name = customer.Name,
            Debt = tracking?.Debt ?? 0,
        };
    }
}
