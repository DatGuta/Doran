using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.CustomerDebtHandlers.Queries;

public class ListOrderCustomerDebtQuery : Request<List<OrderCustomerDebtDto>> {
    public string CustomerId { get; set; } = string.Empty;
    public string RecieptId { get; set; } = string.Empty;
    public DateTimeOffset? ReceiptDate { get; set; }
}

internal class ListOrderCustomerDebtHandler(IServiceProvider serviceProvider)
    : BaseHandler<ListOrderCustomerDebtQuery, List<OrderCustomerDebtDto>>(serviceProvider) {

    public override async Task<List<OrderCustomerDebtDto>> Handle(ListOrderCustomerDebtQuery request, CancellationToken cancellationToken) {
        var handleOrderStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Ticket, EOrderStatus.Export, EOrderStatus.Exported };
        var handlePaymentStatus = new List<EOrderPaymentStatus> { EOrderPaymentStatus.Unpaid, EOrderPaymentStatus.PartialPaid };

        var reciepts = new Dictionary<string, decimal>();
        if (!string.IsNullOrWhiteSpace(request.RecieptId)) {
            reciepts = await db.ReceiptDetails.AsNoTracking()
                .Where(o => o.ReceiptId == request.RecieptId)
                .ToDictionaryAsync(k => k.OrderId, v => v.Value, cancellationToken);
        }
        var orderIdsInReciepts = reciepts.Keys;

        var orders = await db.Orders.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.CustomerId == request.CustomerId
                && handleOrderStatus.Contains(o.Status)
                && (handlePaymentStatus.Contains(o.PaymentStatus) && o.Remaining > decimal.Zero || orderIdsInReciepts.Contains(o.Id)))
            .WhereIf(request.ReceiptDate.HasValue, o => o.CreatedDate <= request.ReceiptDate)
            .ToListAsync(cancellationToken);

        var orderIds = orders.Select(o => o.Id).ToList();
        var orderIdsHasExport = await db.WarehouseExports.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && orderIds.Contains(o.OrderId!) && o.Status == EWarehouseExport.Completed)
            .Select(o => o.OrderId!).Distinct().ToListAsync(cancellationToken);

        return orders.Select(o => OrderCustomerDebtDto.FromEntity(o, orderIdsHasExport))
            .OrderBy(o => o.CreatedDate).ToList();
    }
}
