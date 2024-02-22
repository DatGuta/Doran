using System.Globalization;

namespace DR.ManageApi.Domain.Services.Interfaces;

public interface ICultureInfoService {

    Task<CultureInfo> GetCultureInfo(string merchantId, CancellationToken cancellationToken = default);
}
