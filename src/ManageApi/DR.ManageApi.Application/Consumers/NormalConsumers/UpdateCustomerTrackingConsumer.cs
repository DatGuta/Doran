using DR.Common.Lock;
using DR.Contexts.NormalContexts;
using DR.Database.Models;

namespace DR.ManageApi.Application.Consumers.NormalConsumers;

public class UpdateCustomerTrackingConsumer(IServiceProvider serviceProvider)
    : BaseRabbitMqConsumer<UpdateCustomerTrackingContext>(serviceProvider) {

    public override async Task Handle(UpdateCustomerTrackingContext context, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(context.MerchantId)
                || string.IsNullOrWhiteSpace(context.CustomerId)) {
            return;
        }

        await Locker.LockByKey($"{nameof(UpdateCustomerTrackingConsumer)}:{context.MerchantId}:{context.CustomerId}",
            () => Process(context, cancellationToken), expirySec: 300);
    }

    private async Task Process(UpdateCustomerTrackingContext context, CancellationToken cancellationToken) {
        using var transaction = await this.db.Database.BeginTransactionAsync(cancellationToken);
        try {
            var nonUpdateItem = await this.db.CustomerTrackings.AsNoTracking()
                .Where(o => o.MerchantId == context.MerchantId
                    && o.CustomerId == context.CustomerId
                    && !o.IsUpdate)
                .OrderBy(o => o.Date)
                .FirstOrDefaultAsync(cancellationToken);
            if (nonUpdateItem == null) return;

            var customer = await this.db.Customers.AsNoTracking()
                .Where(o => o.MerchantId == context.MerchantId
                    && o.Id == context.CustomerId)
                .FirstOrDefaultAsync(cancellationToken);
            if (customer == null) return;

            var debtTrackings = await this.db.CustomerTrackings
                .Where(o => o.MerchantId == context.MerchantId
                    && o.CustomerId == context.CustomerId
                    && (nonUpdateItem.Date <= o.Date || o.Id == nonUpdateItem.Id))
                .OrderBy(o => o.Date)
                    .ThenBy(o => o.DocumentType)
                    .ThenBy(o => o.DocumentCode.Length)
                    .ThenBy(o => o.DocumentCode)
                .ToListAsync(cancellationToken);

            var lastDt = await this.db.CustomerTrackings.AsNoTracking()
                .Where(o => o.MerchantId == context.MerchantId
                    && o.CustomerId == context.CustomerId
                    && o.IsUpdate
                    && o.Date < nonUpdateItem.Date)
                .OrderByDescending(o => o.Date)
                .Select(o => new { o.DebtAfter, o.BalanceAfter, o.Date })
                .FirstOrDefaultAsync(cancellationToken);

            HandleCustomerDebtTracking(debtTrackings,
                lastDt?.DebtAfter ?? decimal.Zero,
                lastDt?.BalanceAfter ?? decimal.Zero,
                lastDt?.Date ?? DateTimeOffset.UnixEpoch);

            await this.db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        } catch {
            await transaction.RollbackAsync(cancellationToken);
        }
    }

    private static void HandleCustomerDebtTracking(List<CustomerTracking> debtTrackings, decimal debtAfter, decimal balanceAfter, DateTimeOffset dateAfter) {
        foreach (var debtTracking in debtTrackings) {
            debtTracking.DebtBefore = debtAfter;
            debtTracking.BalanceBefore = balanceAfter;

            debtAfter = CalculateDebt(debtAfter, debtTracking);
            balanceAfter = CalculateBalance(balanceAfter, debtTracking);

            debtTracking.DebtAfter = debtAfter;
            debtTracking.BalanceAfter = balanceAfter;

            debtTracking.IsUpdate = true;

            if (debtTracking.Date <= dateAfter) {
                debtTracking.Date = dateAfter.AddMilliseconds(1);
            }
            dateAfter = debtTracking.Date;
        }
    }

    private static decimal CalculateDebt(decimal debt, CustomerTracking debtTracking) {
        if (debtTracking.IsDelete) {
            return debt;
        }

        return debt + debtTracking.Debt - debtTracking.Balance;
    }

    private static decimal CalculateBalance(decimal balance, CustomerTracking debtTracking) {
        if (debtTracking.IsDelete) {
            return balance;
        }

        return balance + debtTracking.Balance - debtTracking.Debt;
    }
}
