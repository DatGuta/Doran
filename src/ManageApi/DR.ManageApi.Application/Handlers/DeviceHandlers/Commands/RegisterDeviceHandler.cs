using DR.Common.Exceptions;
using DR.Database.Models;
using DR.Helper;

namespace DR.ManageApi.Application.Handlers.DeviceHandlers.Commands;

public class RegisterDeviceCommand : Request<RegisterDeviceData> {
    public string SerialNumber { get; set; } = null!;
    public string Name { get; set; } = null!;

    public string? StoreId { get; set; }
    public string? WarehouseId { get; set; }
}

public class RegisterDeviceData {
    public string DeviceName { get; set; } = string.Empty;
}

internal class RegisterDeviceHandler(IServiceProvider serviceProvider) : BaseHandler<RegisterDeviceCommand, RegisterDeviceData>(serviceProvider) {

    public override async Task<RegisterDeviceData> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken) {
        var devices = await db.Devices
            .Where(o => o.MerchantId == request.MerchantId)
            .Where(o => o.SerialNumber == request.SerialNumber || o.Name == request.Name)
            .ToListAsync(cancellationToken);

        var device = devices.FirstOrDefault(o => o.SerialNumber == request.SerialNumber);
        var checkDevice = devices.FirstOrDefault(o => o.Name == request.Name);

        if (device == null) {
            ManagedException.ThrowIf(checkDevice != null, "Tên thiết bị đã tồn tại.");

            device = new Device {
                Id = NGuidHelper.New(),
                MerchantId = request.MerchantId,
                SerialNumber = request.SerialNumber,
                Name = request.Name,
                IsActive = true,
                StoreId = request.StoreId,
                WarehouseId = request.WarehouseId,
            };
            await db.Devices.AddAsync(device, cancellationToken);
        } else {
            ManagedException.ThrowIf(checkDevice != null && device.Id != checkDevice.Id, "Tên thiết bị đã tồn tại.");

            device.IsActive = true;
            device.StoreId = request.StoreId;
            device.WarehouseId = request.WarehouseId;
        }
        await db.SaveChangesAsync(cancellationToken);

        return new RegisterDeviceData() {
            DeviceName = device.Name,
        };
    }
}
