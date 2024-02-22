using DR.Constant.Enums;
using DR.Database.ExtendModels;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Models.Dto;
using DR.Redis;

namespace DR.ManageApi.Domain.Services.Implements;

public class GlobalSettingService(IServiceProvider serviceProvider) : IGlobalSettingService {
    private readonly DrContext db = serviceProvider.GetRequiredService<DrContext>();
    private readonly IRedisService redisService = serviceProvider.GetRequiredService<IRedisService>();
    private readonly ISettingService settingService = serviceProvider.GetRequiredService<ISettingService>();

    public async Task CleanByMerchantId(string merchantId) {
        var cacheKey = RedisKey.GetGlobalSettingKey(merchantId);
        await this.redisService.RemoveAsync(cacheKey);
    }

    public async Task<GlobalSettingDto> GetByMerchantId(string merchantId, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(merchantId)) {
            return GlobalSettingDto.Default();
        }

        var cacheKey = RedisKey.GetGlobalSettingKey(merchantId);
        if (this.redisService.TryGetValue<GlobalSettingDto>(cacheKey, out var value) && value != null) {
            return value;
        }

        var merchant = await this.db.Merchants.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == merchantId, cancellationToken);
        if (merchant == null) {
            return GlobalSettingDto.Default();
        }

        var numberFormat = await this.settingService.GetValue(merchant.Id, ESetting.NumberFormat, new NumberFormat(), cancellationToken);

        var result = new GlobalSettingDto {
            NumberFormat = numberFormat,
        };

        cacheKey = RedisKey.GetGlobalSettingKey(merchant.Id);
        await this.redisService.SetAsync(cacheKey, result);

        return result;
    }
}
