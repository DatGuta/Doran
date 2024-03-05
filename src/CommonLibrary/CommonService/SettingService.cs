using DR.Constant.Enums;
using DR.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace TuanVu.CommonServices {

    public interface ISettingService {

        Task<JToken> GetValue(ESetting setting, Guid merchantId, CancellationToken cancellationToken = default);

        Task<T?> GetValue<T>(ESetting setting, Guid merchantId, CancellationToken cancellationToken = default);

        Task<T> GetValue<T>(ESetting setting, Guid merchantId, T defaultValue, CancellationToken cancellationToken = default);
    }

    public class SettingService : ISettingService {
        private readonly DrContext db;

        public SettingService(IServiceProvider serviceProvider) {
            this.db = serviceProvider.GetRequiredService<DrContext>();
        }

        public async Task<JToken> GetValue(ESetting setting, Guid merchantId, CancellationToken cancellationToken = default) {
            var merchantSetting = await this.db.MerchantSettings.AsNoTracking()
                .Where(o => o.Code == setting && o.MerchantId == merchantId)
                .Select(o => new { o.Value })
                .FirstOrDefaultAsync(cancellationToken);
            if (merchantSetting != null)
                return merchantSetting.Value;

            var generalSetting = await this.db.GeneralSettings.AsNoTracking()
                .Where(o => o.Code == setting)
                .Select(o => new { o.DefaultValue })
                .FirstOrDefaultAsync(cancellationToken);
            if (generalSetting != null)
                return generalSetting.DefaultValue;

            return JValue.CreateNull();
        }

        public async Task<T?> GetValue<T>(ESetting setting, Guid merchantId, CancellationToken cancellationToken = default) {
            var value = await this.GetValue(setting, merchantId, cancellationToken);
            return value.ToObject<T>();
        }

        public async Task<T> GetValue<T>(ESetting setting, Guid merchantId, T defaultValue, CancellationToken cancellationToken = default) {
            var value = await this.GetValue<T>(setting, merchantId, cancellationToken);
            return value ?? defaultValue;
        }
    }
}
