using DR.Constant.Enums;
using DR.Database.Models;

namespace FMS.ManageApi.Application.Handlers.CustomerDebtHandlers.Queries;

public class ListCustomerDebtQuery : PaginatedRequest<ListDebtData> {
    public string CustomerId { get; set; } = string.Empty;
}

public class ListDebtData : PaginatedList<DebtDto> { }

internal class ListCustomerDebtHandler(IServiceProvider serviceProvider)
    : BaseHandler<ListCustomerDebtQuery, ListDebtData>(serviceProvider) {

    public override async Task<ListDebtData> Handle(ListCustomerDebtQuery request, CancellationToken cancellationToken) {
        var query = db.Debts.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.CustomerId == request.CustomerId && !o.IsDelete
                && o.ItemType == EDebtItem.OrderPayment);

        var debts = await query.OrderByDescending(o => o.Time)
            .Skip(request.Skip).Take(request.Take)
            .Select(o => DebtDto.FromEntity(o)).ToListAsync(cancellationToken);

        if (debts.Count > 0) {
            var debtIds = debts.Select(o => o.Id).ToList();
            var debtDetails = await db.DebtDetails.AsNoTracking()
                .Where(o => debtIds.Contains(o.DebtId)).ToListAsync(cancellationToken);

            var orderPayIds = debtDetails.Select(o => o.ItemId).Distinct().ToList();

            var orderPays = new List<OrderPayment>();
            if (orderPayIds.Count > 0) {
                orderPays = await db.OrderPayments.AsNoTracking().Include(o => o.Order)
                    .Where(o => orderPayIds.Contains(o.Id)).ToListAsync(cancellationToken);
            }

            foreach (var debt in debts) {
                debt.Items = debtDetails.Where(o => o.DebtId == debt.Id)
                    .OrderByDescending(o => o.Time)
                    .Select(item => {
                        var order = orderPays.FirstOrDefault(o => o.Id == item.ItemId)?.Order;
                        return DebtItemDto.FromEntity(item, order);
                    }).ToList();
            }
        }

        return new() {
            Items = debts,
            Count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken),
        };
    }
}
