using DR.ManageApi.Application.Handlers;

namespace TuanVu.Handlers.OrderHandlers;
public class QuickExportOrderReq : ModelRequest<List<string>> { }

public class QuickExportOrderHandler : BaseHandler<QuickExportOrderReq> {

    public QuickExportOrderHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
    }

    public override async Task Handle(QuickExportOrderReq request, CancellationToken cancellationToken) {
        var merchantId = request.MerchantId;
        var userId = request.UserId;
        var orderIds = request.Model;

        var items = await this.db.OrderDetails.AsNoTracking()
            .Where(o => orderIds.Contains(o.OrderId) && o.Quantity > o.ExportQuantity)
            .Select(o => new {
                o.Id,
                o.OrderId,
                Qty = o.Quantity - o.ExportQuantity,
            }).ToListAsync(cancellationToken);

        var reqs = items.GroupBy(o => o.OrderId)
            .Select(o => new ExportOrderReq {
                MerchantId = merchantId,
                UserId = userId,
                OrderId = o.Key,
                Items = o.Select(x => new ExportOrderItem {
                    ItemId = x.Id,
                    Quantity = x.Qty
                }).ToList(),
            }).ToList();

        foreach (var req in reqs) {
            await mediator.Send(req, cancellationToken);
        }
    }
}
}
