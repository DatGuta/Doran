using DR.Constant.Enums;
using DR.Resource;

namespace DR.ManageApi.Application.Handlers.DashboardHandlers.Queries;

public class SummarySaleByProvinceChartQuery : Request<List<SummarySaleByProvinceChartItem>> {
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
}

public record SummarySaleByProvinceChartItem {
    public string BrandName { get; set; } = "";
    public string ProvinceName { get; set; } = "";
    public decimal TotalQuantity { get; set; }
}

internal class SummarySaleByProvinceChartDashboardHandler(IServiceProvider serviceProvider)
    : BaseHandler<SummarySaleByProvinceChartQuery, List<SummarySaleByProvinceChartItem>>(serviceProvider) {

    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<List<SummarySaleByProvinceChartItem>> Handle(SummarySaleByProvinceChartQuery request, CancellationToken cancellationToken) {
        var handleOrderStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Export, EOrderStatus.Exported };

        var query = from ordDetail in db.OrderDetails.AsNoTracking()
                    join order in db.Orders.AsNoTracking() on ordDetail.OrderId equals order.Id
                    join orderCustomer in db.OrderCustomers.AsNoTracking() on order.Id equals orderCustomer.OrderId
                    join prodBrand in db.ProductBrands.AsNoTracking() on ordDetail.ProductId equals prodBrand.ProductId into pb
                    from prdBrd in pb.DefaultIfEmpty()
                    where order.MerchantId == request.MerchantId && order.Customer != null
                             && request.From <= order.CreatedDate && order.CreatedDate < request.To
                             && order.Type == EOrder.Normal && handleOrderStatus.Contains(order.Status)
                             && !ordDetail.IsPromotion
                    select new {
                        BrandId = prdBrd != null ? prdBrd.BrandId : "",
                        ProvinceId = orderCustomer != null ? orderCustomer.Province : "",
                        ordDetail.Quantity
                    };

        var items = await query.GroupBy(o => new { o.BrandId, o.ProvinceId })
                                .Select(o => new { o.Key.BrandId, o.Key.ProvinceId, Quantity = o.Sum(x => x.Quantity) })
                                .Where(o => !string.IsNullOrWhiteSpace(o.BrandId) && !string.IsNullOrWhiteSpace(o.ProvinceId))
                                .ToListAsync(cancellationToken);

        var brandIds = items.Select(o => o.BrandId).Distinct().ToList();
        var brandMap = await db.Brands.AsNoTracking()
                                    .Where(o => o.MerchantId == request.MerchantId && brandIds.Contains(o.Id) && !o.IsDelete)
                                    .ToDictionaryAsync(k => k.Id, cancellationToken);

        var provinceIds = items.Where(o => !string.IsNullOrWhiteSpace(o.ProvinceId)).Select(o => o.ProvinceId).Distinct().ToArray();
        var provinceMap = unitRes.GetByCode(provinceIds);

        return items.Select(x => {
            var brand = brandMap.GetValueOrDefault(x.BrandId);
            var province = provinceMap.GetValueOrDefault(x.ProvinceId);
            return new SummarySaleByProvinceChartItem {
                BrandName = !string.IsNullOrWhiteSpace(brand?.Name) ? brand.Name : "Khác",
                ProvinceName = !string.IsNullOrWhiteSpace(province?.Name) ? province.Name : "Khác",
                TotalQuantity = x.Quantity
            };
        }).ToList();
    }
}
