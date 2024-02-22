using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.DashboardHandlers.Queries;

public class NearlySoldOutDashboardReq : Request<List<NearlySoldOutDto>> {
    public int Top { get; set; }
    public List<string> WarehouseIds { get; set; } = [];
    public List<string> SupplierIds { get; set; } = [];
}

internal class NearlySoldOutDashboardHandler(IServiceProvider serviceProvider)
    : BaseHandler<NearlySoldOutDashboardReq, List<NearlySoldOutDto>>(serviceProvider) {

    public override async Task<List<NearlySoldOutDto>> Handle(NearlySoldOutDashboardReq request, CancellationToken cancellationToken) {
        if (request.Top < 0 || request.Top > 50
            || request.WarehouseIds?.Count == 0)
            return [];

        var query = db.ProductOnWarehouses.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.IsActive && request.WarehouseIds!.Contains(o.WarehouseId))
            .Where(o => o.Product!.MerchantId == request.MerchantId && !o.Product!.IsDelete)
            .Where(o => o.Warehouse!.MerchantId == request.MerchantId && !o.Warehouse!.IsDelete
                && o.Warehouse!.IsActive && o.Warehouse!.Type == EWarehouse.Normal);

        if (request.SupplierIds != null && request.SupplierIds.Count > 0) {
            var queryProductIds = db.ProductSuppliers.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && request.SupplierIds.Contains(o.SupplierId) && !o.IsDelete)
                .Select(o => o.ProductId).Distinct();
            query = query.Where(o => queryProductIds.Contains(o.ProductId));
        }

        var topProductIds = await query.GroupBy(o => o.ProductId)
            .Select(o => new { o.Key, OnHand = o.Sum(x => x.OnHand) })
            .OrderBy(o => o.OnHand).Take(request.Top)
            .Select(o => o.Key).ToListAsync(cancellationToken);

        return await query.Where(o => topProductIds.Contains(o.ProductId))
            .OrderBy(o => o.OnHand)
            .Select(o => new NearlySoldOutDto {
                ProductCode = o.Product!.Code,
                Product = o.Product!.Name,
                Warehouse = o.Warehouse!.Name,
                Value = o.OnHand,
            }).ToListAsync(cancellationToken);
    }
}
