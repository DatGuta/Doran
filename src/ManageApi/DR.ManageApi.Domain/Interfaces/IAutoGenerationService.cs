using DR.Constant.Enums;
using DR.ManageApi.Domain.Actions.AutoGenerate;

namespace DR.ManageApi.Domain.Services.Interfaces;

public interface IAutoGenerationService {

    Task<string> GenerateCode(string merchantId, ESetting setting, EAutoGenerator action, CancellationToken cancellationToken);
}
