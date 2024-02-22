using DR.Models.Dto;

namespace DR.ManageApi.Domain.Services.Interfaces;

public interface IGlobalSettingService {

    Task CleanByMerchantId(string merchantId);

    Task<GlobalSettingDto> GetByMerchantId(string merchantId, CancellationToken cancellationToken = default);
}
