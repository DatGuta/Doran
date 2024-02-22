using FMS.Constant.Enums;
using FMS.ManageApi.Domain.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace FMS.ManageApi.Domain.Services.Implements;

public class SettingService(IServiceProvider serviceProvider) : ISettingService {
    private readonly FmsContext db = serviceProvider.GetRequiredService<FmsContext>();

    public async Task<JToken> GetValue(string merchantId, ESetting setting, CancellationToken cancellationToken = default) {
        var merchantSetting = await this.db.MerchantSettings.AsNoTracking()
            .Where(o => o.Code == setting && o.MerchantId == merchantId)
            .FirstOrDefaultAsync(cancellationToken);
        if (merchantSetting != null)
            return merchantSetting.Value;

        var generalSetting = await this.db.GeneralSettings.AsNoTracking()
            .Where(o => o.Code == setting)
            .FirstOrDefaultAsync(cancellationToken);
        if (generalSetting != null)
            return generalSetting.DefaultValue;

        return JValue.CreateNull();
    }

    public async Task<T?> GetValue<T>(string merchantId, ESetting setting, CancellationToken cancellationToken = default) {
        var value = await this.GetValue(merchantId, setting, cancellationToken);
        return value.ToObject<T>();
    }

    public async Task<T> GetValue<T>(string merchantId, ESetting setting, T defaultValue, CancellationToken cancellationToken = default) {
        var value = await this.GetValue<T>(merchantId, setting, cancellationToken);
        return value ?? defaultValue;
    }
}
