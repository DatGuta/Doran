using DR.Common.Exceptions;
using DR.Configuration;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.MerchantHandlers.Queries;

public class GetApiKeyMerchantQuery : Request<ApiKeyData> { }

internal class GetApiKeyMerchantHandler(IServiceProvider serviceProvider) : BaseHandler<GetApiKeyMerchantQuery, ApiKeyData>(serviceProvider) {

    public override async Task<ApiKeyData> Handle(GetApiKeyMerchantQuery request, CancellationToken cancellationToken) {
        var merchant = await db.Merchants.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.MerchantId, cancellationToken);
        ManagedException.ThrowIf(merchant == null, Messages.Merchant.GetApiKey.Merchant_NotFound);

        return new ApiKeyData {
            ApiKey = !string.IsNullOrWhiteSpace(merchant.ApiSecret) ? merchant.Id : string.Empty,
            ApiSecret = merchant.ApiSecret ?? string.Empty,
        };
    }
}
