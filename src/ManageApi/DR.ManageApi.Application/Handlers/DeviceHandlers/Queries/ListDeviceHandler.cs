using DR.Resource;

namespace DR.ManageApi.Application.Handlers.DeviceHandlers.Queries;
public class ListDeviceQuery : PaginatedRequest<ListDeviceData> { }

public class ListDeviceData : PaginatedList<DeviceDto> { }


internal class ListDeviceHandler(IServiceProvider serviceProvider) : BaseHandler<ListDeviceQuery, ListDeviceData>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<ListDeviceData> Handle(ListDeviceQuery request, CancellationToken cancellationToken) {
        var query = db.Devices.AsNoTracking().Where(o => o.MerchantId == request.MerchantId);

        var devices = await query.OrderByDescending(o => o.CreatedDate)
            .Skip(request.PageIndex * request.PageSize).Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var storeIds = devices.Where(o => !string.IsNullOrWhiteSpace(o.StoreId)).Select(o => o.StoreId!).Distinct().ToList();
        var stores = new List<StoreDto>();
        if (storeIds.Count > 0) {
            stores = await db.Stores.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && storeIds.Contains(o.Id))
                .Select(o => StoreDto.FromEntity(o, unitRes))
                .ToListAsync(cancellationToken);
        }

        var warehouseIds = devices.Where(o => !string.IsNullOrWhiteSpace(o.WarehouseId)).Select(o => o.WarehouseId!).Distinct().ToList();
        var warehouses = new List<WarehouseDto>();
        if (warehouseIds.Count > 0) {
            warehouses = await db.Warehouses.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && storeIds.Contains(o.Id))
                .Select(o => WarehouseDto.FromEntity(o, unitRes, false))
                .ToListAsync(cancellationToken);
        }

        var items = new List<DeviceDto>();
        foreach (var device in devices) {
            var store = !string.IsNullOrEmpty(device.StoreId) ? stores.FirstOrDefault(o => o.Id == device.StoreId) : null;
            var warehouse = !string.IsNullOrEmpty(device.WarehouseId) ? warehouses.FirstOrDefault(o => o.Id == device.WarehouseId) : null;
            items.Add(DeviceDto.FromEntity(device, store, warehouse));
        }

        return new() {
            Items = items,
            Count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken),
        };
    }
}
