using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.CustomerRpHandlers.Queries;

public class ListDebtDetailQuery : PaginatedRequest<ListDebtDetaiData> {
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public bool IsAll { get; set; }
}

public class ListDebtDetaiData : PaginatedList<DebtDetailDto> { }

internal class ListDebtDetailHandler(IServiceProvider serviceProvider) : BaseHandler<ListDebtDetailQuery, ListDebtDetaiData>(serviceProvider) {

    public override async Task<ListDebtDetaiData> Handle(ListDebtDetailQuery request, CancellationToken cancellationToken) {
        var query = db.CustomerTrackings.AsNoTracking()
            .OrderByDescending(o => o.Date)
            .Where(o => o.MerchantId == request.MerchantId && o.CustomerId == request.CustomerId && o.IsUpdate && !o.IsDelete)
            .WhereIf(request.FromDate.HasValue, o => request.FromDate <= o.Date)
            .WhereIf(request.ToDate.HasValue, o => o.Date <= request.ToDate);

        int count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);

        if (!request.IsAll) {
            query = query.Skip(request.Skip).Take(request.Take);
        }

        var trackings = await query.ToListAsync(cancellationToken);

        var payments = new Dictionary<string, Tuple<string, string>>();
        var paymentTypes = new List<ECustomerDocType> { ECustomerDocType.PaymentRefund, ECustomerDocType.PaymentStandard };
        var paymentIds = trackings.Where(o => paymentTypes.Contains(o.DocumentType)).Select(o => o.DocumentId).Distinct().ToList();
        if (paymentIds.Count > 0) {
            payments = await db.Payments.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && paymentIds.Contains(o.Id))
                .ToDictionaryAsync(k => k.Id, v => Tuple.Create(v.Code, v.Description), cancellationToken);
        }

        var receipts = new Dictionary<string, Tuple<string, string>>();
        var receiptIds = trackings.Where(o => o.DocumentType == ECustomerDocType.Receipt).Select(o => o.DocumentId).Distinct().ToList();
        if (receiptIds.Count > 0) {
            receipts = await db.Receipts.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && receiptIds.Contains(o.Id))
                .ToDictionaryAsync(k => k.Id, v => Tuple.Create(v.Code, v.Description), cancellationToken);
        }

        var orderRefunds = new Dictionary<string, Tuple<string, string>>();
        var orderRefundIds = trackings.Where(o => o.DocumentType == ECustomerDocType.OrderRefund).Select(o => o.DocumentId).Distinct().ToList();
        if (orderRefundIds.Count > 0) {
            orderRefunds = await db.OrderActions.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && orderRefundIds.Contains(o.Id))
                .ToDictionaryAsync(k => k.Id, v => Tuple.Create(v.Code, string.Empty), cancellationToken);
        }

        var orders = new Dictionary<string, Tuple<string, string?>>();
        var orderIds = trackings.Where(o => o.DocumentType == ECustomerDocType.Order).Select(o => o.DocumentId).Distinct().ToList();
        if (orderIds.Count > 0) {
            orders = await db.Orders.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && orderIds.Contains(o.Id))
                .ToDictionaryAsync(k => k.Id, v => Tuple.Create(v.OrderNo, v.Description), cancellationToken);
        }

        var items = trackings.Select(o => {
            var item = new DebtDetailDto {
                Date = o.Date,
                ItemId = o.DocumentId,
                Code = o.DocumentCode,
            };

            switch (o.DocumentType) {
                case ECustomerDocType.Order:
                    item.Type = EDebtDetailType.Order;
                    item.Debt = o.Debt;
                    item.Code = orders.GetValueOrDefault(o.DocumentId)?.Item1 ?? string.Empty;
                    item.Reason = orders.GetValueOrDefault(o.DocumentId)?.Item2 ?? string.Empty;
                    break;

                case ECustomerDocType.OrderRefund:
                    item.Type = EDebtDetailType.Refund;
                    item.Debt = o.Debt;
                    item.Code = orderRefunds.GetValueOrDefault(o.DocumentId)?.Item1 ?? string.Empty;
                    break;

                case ECustomerDocType.Receipt:
                    item.Type = EDebtDetailType.Receipt;
                    item.Balance = o.Balance;
                    item.Code = receipts.GetValueOrDefault(o.DocumentId)?.Item1 ?? string.Empty;
                    item.Reason = receipts.GetValueOrDefault(o.DocumentId)?.Item2 ?? string.Empty;
                    break;

                case ECustomerDocType.PaymentStandard:
                case ECustomerDocType.PaymentRefund:
                    item.Type = EDebtDetailType.Payment;
                    item.Balance = o.Balance;
                    item.Code = payments.GetValueOrDefault(o.DocumentId)?.Item1 ?? string.Empty;
                    item.Reason = payments.GetValueOrDefault(o.DocumentId)?.Item2 ?? string.Empty;
                    break;
            }

            item.DebtBefore = o.DebtBefore > 0 ? o.DebtBefore : 0;
            item.BalanceBefore = o.BalanceBefore > 0 ? o.BalanceBefore : 0;
            item.DebtAfter = o.DebtAfter > 0 ? o.DebtAfter : 0;
            item.BalanceAfter = o.BalanceAfter > 0 ? o.BalanceAfter : 0;

            return item;
        }).ToList();

        return new() {
            Count = count,
            Items = items,
        };
    }
}
