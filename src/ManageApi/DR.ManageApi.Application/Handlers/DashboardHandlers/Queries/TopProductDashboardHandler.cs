using DR.Constant.Enums;
using DR.Helper;

namespace DR.ManageApi.Application.Handlers.DashboardHandlers.Queries;

public class TopProductQuery : Request<List<TopProductDto>> {
    public DateTimeOffset Time { get; set; }
    public int Top { get; set; }
    public EDateTimePeriod Period { get; set; }
}

internal class TopProductDashboardHandler(IServiceProvider serviceProvider)
    : BaseHandler<TopProductQuery, List<TopProductDto>>(serviceProvider) {

    public override async Task<List<TopProductDto>> Handle(TopProductQuery request, CancellationToken cancellationToken) {
        if (request.Top < 1) request.Top = 10;

        var handleOrderStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Export, EOrderStatus.Exported };
        var (fromTime, toTime) = DateTimeHelper.GetPeriod(request.Time, request.Period);
        var items = await db.OrderDetails.AsNoTracking()
            .Where(o => o.Order!.MerchantId == request.MerchantId
                && fromTime <= o.Order!.CreatedDate && o.Order!.CreatedDate < toTime
                && handleOrderStatus.Contains(o.Order!.Status) && !o.IsPromotion)
            .GroupBy(o => o.ProductId!)
            .Select(o => new TopProductDto {
                Id = o.Key,
                Quantity = o.Sum(x => x.Quantity),
                Frequency = o.Select(x => x.OrderId).Distinct().Count(),
            }).OrderByDescending(o => o.Quantity)
            .Take(request.Top).ToListAsync(cancellationToken);

        var productIds = items.Select(o => o.Id).ToList();
        var products = await db.Products.Where(o => o.MerchantId == request.MerchantId && productIds.Contains(o.Id))
            .AsNoTracking().ToDictionaryAsync(k => k.Id, cancellationToken);

        items.ForEach(item => item.Name = products.GetValueOrDefault(item.Id)?.Name ?? string.Empty);
        return items;
    }
}
