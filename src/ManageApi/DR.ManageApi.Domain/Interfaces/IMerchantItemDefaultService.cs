using DR.Constant.Enums;

namespace DR.ManageApi.Domain.Services.Interfaces;

public interface IMerchantItemDefaultService {

    Task<Dictionary<EMerchantItemDefault, string?>> List(string merchantId, List<EMerchantItemDefault>? types = null, CancellationToken cancellationToken = default);

    Task<string?> Get(string merchantId, EMerchantItemDefault itemType, CancellationToken cancellationToken = default);

    Task Save(string merchantId, Dictionary<EMerchantItemDefault, string?> items, CancellationToken cancellationToken = default);
}
