using System.Globalization;
using DR.Constant.Enums;
using DR.Database;
using DR.Database.ExtendModels;
using DR.Operators.AutoGenerate;
using Microsoft.Extensions.DependencyInjection;
using TuanVu.CommonServices;

namespace DR.CommonServices {

    public interface IAutoGenerationService {

        Task<string> GenerateCode(Guid merchantId, ESetting setting, CancellationToken cancellationToken);

        Task<string[]> GenerateCode(Guid merchantId, ESetting setting, int numberOfCode, CancellationToken cancellationToken);
    }

    public class AutoGenerationService : IAutoGenerationService {
        private readonly DrDbContext db;
        private readonly ISettingService settingService;
        private readonly Random random;

        public AutoGenerationService(IServiceProvider serviceProvider) {
            this.db = serviceProvider.GetRequiredService<DrDbContext>();
            this.settingService = serviceProvider.GetRequiredService<ISettingService>();
            this.random = new Random();
        }

        public async Task<string> GenerateCode(Guid merchantId, ESetting setting, CancellationToken cancellationToken) {
            var arr = await this.GenerateCode(merchantId, setting, 1, cancellationToken);
            return arr.FirstOrDefault() ?? string.Empty;
        }

        public async Task<string[]> GenerateCode(Guid merchantId, ESetting setting, int numberOfCode, CancellationToken cancellationToken) {
            var autoGenerateSetting = await this.settingService.GetValue<AutoGenerate>(setting, merchantId, cancellationToken);
            if (autoGenerateSetting == null) return Array.Empty<string>();

            var prefix = GetPrefix(autoGenerateSetting);
            var number = await GetLastNumber(setting, prefix, merchantId);

            string[] arr = new string[numberOfCode];
            for (int i = 0; i < numberOfCode; i++) {
                int incrNumber = number == 0 ? autoGenerateSetting.StartNumber : GetIncrNumber(autoGenerateSetting);
                number += incrNumber;

                arr[i] = $"{prefix}{number:000}";
            }
            return arr;
        }

        private int GetIncrNumber(AutoGenerate setting) {
            if (!setting.IsRandomIncrNumber || setting.IncrNumber == 1)
                return setting.IncrNumber;

            return this.random.Next(1, setting.IncrNumber + 1);
        }

        private static string GetPrefix(AutoGenerate setting) {
            if (!setting.ResetBy.HasValue) return $"{setting.Prefix}_";
            var dateTimeOffset = DateTimeOffset.Now;

            if (setting.ResetBy == EDateTimePeriod.Week) {
                var dfi = DateTimeFormatInfo.CurrentInfo;
                var numbWeek = dfi.Calendar.GetWeekOfYear(dateTimeOffset.Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                return $"{setting.Prefix}-{dateTimeOffset.Date:yyyy}W{numbWeek:00}-";
            }

            return setting.ResetBy switch {
                EDateTimePeriod.Day => $"{setting.Prefix}-{dateTimeOffset.Date:yyyyMMdd}-",
                EDateTimePeriod.Month => $"{setting.Prefix}-{dateTimeOffset.Date:yyyyMM}-",
                EDateTimePeriod.Year => $"{setting.Prefix}-{dateTimeOffset.Date:yyyy}-",
                null => "",
                _ => "",
            };
        }

        private async Task<int> GetLastNumber(ESetting settingType, string prefix, Guid merchantId) {
            var op = AutoGenerateFactory.Create(settingType);
            return await GetLastCode(op, merchantId, prefix, new List<string>());
        }

        private async Task<int> GetLastCode(IAutoGenerateOperator op, Guid merchantId, string prefix, List<string> excludes) {
            var code = await op.GetLastCode(this.db, merchantId, prefix, excludes);
            if (string.IsNullOrWhiteSpace(code))
                return 0;

            var codeSegments = code.Split(prefix);
            if (codeSegments.Length >= 2 && int.TryParse(codeSegments[1], out int value)) {
                return value;
            }

            excludes.Add(code);
            return await GetLastCode(op, merchantId, prefix, excludes);
        }
    }
}
