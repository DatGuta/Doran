using System.Linq.Dynamic.Core;
using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.DashboardHandlers.Queries;

public class SummarySaleByBrandChartQuery : Request<SummarySaleByBrandData> {
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
}

public class SummarySaleByBrandData {
    public List<SummarySaleByBrandChartItem> SummarySaleByBrandChartData { get; set; } = [];
}

public class SaleProduct {
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public record SummarySaleByBrandChartItem(string Type, decimal Value, List<SaleProduct> Details);

internal class SummarySaleByBrandChartDashboardHandler(IServiceProvider serviceProvider)
    : BaseHandler<SummarySaleByBrandChartQuery, SummarySaleByBrandData>(serviceProvider) {

    public override async Task<SummarySaleByBrandData> Handle(SummarySaleByBrandChartQuery request, CancellationToken cancellationToken) {
        var handleOrderStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Export, EOrderStatus.Exported };

        var queryProdBrd = db.ProductBrands.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete);

        var items = await db.OrderDetails.AsNoTracking()
            .Include(o => o.Product)
            .Include(o => o.Order).ThenInclude(o => o!.Customer)
            .Join(queryProdBrd, od => od.ProductId, ps => ps.ProductId, (od, ps) => new { od, ps })
            .Where(o => o.od.Order!.MerchantId == request.MerchantId
                && request.From <= o.od.Order!.CreatedDate
                && o.od.Order!.CreatedDate < request.To
                && !o.od.IsPromotion
                && o.od.Order!.Type == EOrder.Normal
                && handleOrderStatus.Contains(o.od.Order!.Status))
            .Select(o => new {
                Province = o.od.Order!.Customer != null ? o.od.Order!.Customer.Province : null,
                o.ps.BrandId,
                o.od.Quantity,
                Product = o.od.Product!.Name
            }).GroupBy(o => new { o.Province, o.BrandId })
            .Select(o => new {
                o.Key.Province,
                o.Key.BrandId,
                Quantity = o.Sum(x => x.Quantity),
                Products = o.Select(p => new SaleProduct() {
                    ProductName = p.Product,
                    Quantity = p.Quantity
                }).ToList()
            }).ToListAsync(cancellationToken);

        var brandIds = items.Where(o => !string.IsNullOrWhiteSpace(o.BrandId)).Select(o => o.BrandId!).Distinct().ToList();
        var brandMap = await db.Brands.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && brandIds.Contains(o.Id) && !o.IsDelete)
            .ToDictionaryAsync(k => k.Id, v => v.Name, cancellationToken);

        var brandChartData = items.Where(o => !string.IsNullOrWhiteSpace(o.BrandId))
            .GroupBy(o => o.BrandId!)
            .Select(o => new SummarySaleByBrandChartItem(brandMap.GetValueOrDefault(o.Key)
                      ?? string.Empty, o.Sum(x => x.Quantity), o.SelectMany(x => x.Products).ToList()))
            .OrderByDescending(o => o.Value)
            .ToList();

        return new SummarySaleByBrandData {
            SummarySaleByBrandChartData = brandChartData,
        };
    }
}
