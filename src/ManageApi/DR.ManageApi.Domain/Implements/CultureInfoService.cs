using System.Globalization;
using FMS.ManageApi.Domain.Services.Interfaces;

namespace FMS.ManageApi.Domain.Services.Implements;

public class CultureInfoService(IServiceProvider serviceProvider) : ICultureInfoService {
    private readonly IGlobalSettingService globalSettingService = serviceProvider.GetRequiredService<IGlobalSettingService>();

    public async Task<CultureInfo> GetCultureInfo(string merchantId, CancellationToken cancellationToken = default) {
        var globalSetting = await this.globalSettingService.GetByMerchantId(merchantId, cancellationToken);
        return globalSetting.NumberFormat.GetCultureInfo();
    }
}
