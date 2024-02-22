using DR.Constant.Enums;
using Newtonsoft.Json.Linq;

namespace DR.ManageApi.Domain.Services.Interfaces;

public interface ISettingService {

    Task<JToken> GetValue(string merchantId, ESetting setting, CancellationToken cancellationToken = default);

    Task<T?> GetValue<T>(string merchantId, ESetting setting, CancellationToken cancellationToken = default);

    Task<T> GetValue<T>(string merchantId, ESetting setting, T defaultValue, CancellationToken cancellationToken = default);
}
