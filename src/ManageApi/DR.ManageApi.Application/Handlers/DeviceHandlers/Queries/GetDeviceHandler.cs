namespace DR.ManageApi.Application.Handlers.DeviceHandlers.Queries;

public class GetDeviceQuery : Request<GetDeviceData> {
    public string SerialNumber { get; set; } = null!;

    public string? StoreId { get; set; }
    public string? WarehouseId { get; set; }
}

public class GetDeviceData {
    public string DeviceName { get; set; } = string.Empty;
}

internal class GetDeviceHandler(IServiceProvider serviceProvider) : BaseHandler<GetDeviceQuery, GetDeviceData>(serviceProvider) {

    public override async Task<GetDeviceData> Handle(GetDeviceQuery request, CancellationToken cancellationToken) {
        var deviceName = await db.Devices
            .Where(o => o.MerchantId == request.MerchantId)
            .Where(o => o.SerialNumber == request.SerialNumber)
            .Select(o => o.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return new GetDeviceData() {
            DeviceName = deviceName ?? string.Empty
        };
    }
}
