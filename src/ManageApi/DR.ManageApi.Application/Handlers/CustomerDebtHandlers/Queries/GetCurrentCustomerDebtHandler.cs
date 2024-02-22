using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.CustomerDebtHandlers.Queries;

public class GetCurrentCustomerDebtQuery : Request<CustomerDebtDto> {
    public string CustomerId { get; set; } = string.Empty;
}

internal class GetCurrentCustomerDebtHandler(IServiceProvider serviceProvider)
    : BaseHandler<GetCurrentCustomerDebtQuery, CustomerDebtDto>(serviceProvider) {

    public override async Task<CustomerDebtDto> Handle(GetCurrentCustomerDebtQuery request, CancellationToken cancellationToken) {
        var currentDebt = await db.CustomerTrackings.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.CustomerId == request.CustomerId && o.IsUpdate && !o.IsDelete)
            .OrderByDescending(o => o.Date)
            .Select(o => o.DebtAfter > 0 ? o.DebtAfter : 0)
            .FirstOrDefaultAsync(cancellationToken);

        var accountAmount = await db.CustomerDebts.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.Id == request.CustomerId)
            .Select(o => o.AccountAmount)
            .FirstOrDefaultAsync(cancellationToken);

        var disablePayment = await IsDisablePayment(request.MerchantId, request.CustomerId, cancellationToken);

        return new() {
            AccountAmount = accountAmount,
            CurrentDebt = currentDebt,
            DisableReceipt = currentDebt == 0,
            DisablePayment = disablePayment,
            DisablePaymentRefund = accountAmount == 0,
        };
    }

    private async Task<bool> IsDisablePayment(string merchantId, string customerId, CancellationToken cancellationToken) {
        var handleOrderStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Export, EOrderStatus.Exported };
        bool allowCreatePayment = await db.Orders.AsNoTracking()
            .Where(o => o.MerchantId == merchantId && o.CustomerId == customerId)
            .Where(o => handleOrderStatus.Contains(o.Status) && o.Remaining < 0)
            .Select(o => o.Id)
            .AnyAsync(cancellationToken);

        return !allowCreatePayment;
    }
}
