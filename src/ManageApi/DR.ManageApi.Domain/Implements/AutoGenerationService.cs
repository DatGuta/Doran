using System.Globalization;
using FMS.Constant.Enums;
using FMS.Database.ExtendModels;
using FMS.ManageApi.Domain.Actions.AutoGenerate;
using FMS.ManageApi.Domain.Services.Interfaces;

namespace FMS.ManageApi.Domain.Services.Implements;

public class AutoGenerationService(IServiceProvider serviceProvider) : IAutoGenerationService {
    private readonly FmsContext db = serviceProvider.GetRequiredService<FmsContext>();
    private readonly ISettingService settingService = serviceProvider.GetRequiredService<ISettingService>();
    private readonly Random random = new Random();

    public async Task<string> GenerateCode(string merchantId, ESetting setting, EAutoGenerator action, CancellationToken cancellationToken) {
        var autoGenerateSetting = await this.settingService.GetValue<AutoGenerate>(merchantId, setting, cancellationToken);
        if (autoGenerateSetting == null) return string.Empty;

        var prefix = GetPrefix(autoGenerateSetting);
        var number = await GetLastNumber(action, prefix, merchantId);

        int incrNumber = number == 0 ? autoGenerateSetting.StartNumber : GetIncrNumber(autoGenerateSetting);
        number += incrNumber;

        return $"{prefix}{number:000}";
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

    private int GetIncrNumber(AutoGenerate setting) {
        if (!setting.IsRandomIncrNumber || setting.IncrNumber == 1)
            return setting.IncrNumber;

        return this.random.Next(1, setting.IncrNumber + 1);
    }

    private async Task<int> GetLastNumber(EAutoGenerator action, string prefix, string merchantId) {
        var op = AutoGeneratorFactory.Create(action);
        return await GetLastCode(op, merchantId, prefix, []);
    }

    private async Task<int> GetLastCode(IAutoGenerator op, string merchantId, string prefix, List<string> excludes) {
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
