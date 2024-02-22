using DR.Common.Exceptions;
using DR.Configuration;
using DR.Database.Models;
using DR.Helper;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.MerchantHandlers.Commands;
public class GenerateApiKeyMerchantCommand : Request<ApiKeyData> {
}

internal class GenerateApiKeyMerchantHandler(IServiceProvider serviceProvider) : BaseHandler<GenerateApiKeyMerchantCommand, ApiKeyData>(serviceProvider) {

    public override async Task<ApiKeyData> Handle(GenerateApiKeyMerchantCommand request, CancellationToken cancellationToken) {
        var merchant = await db.Merchants.FirstOrDefaultAsync(o => o.Id == request.MerchantId, cancellationToken);
        ManagedException.ThrowIf(merchant == null, Messages.Merchant.GenerateApiKey.Merchant_NotFound);
        ManagedException.ThrowIf(DateTimeOffset.Now > merchant.ExpiredDate, Messages.Merchant.GenerateApiKey.Merchant_Expired);

        var systemUser = await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.IsSystem, cancellationToken);
        if (systemUser == null) {
            systemUser = new User {
                Id = NGuidHelper.New(),
                MerchantId = request.MerchantId,
                Username = "System",
                Password = "",
                PinCode = "",
                Name = "System",
                SearchName = "System",
                IsSystem = true,
            };
            await db.Users.AddAsync(systemUser, cancellationToken);
        }

        var apikey = new ApiKey(merchant.Id, systemUser.Id);

        merchant.ApiSecret = apikey.GetSecret();
        merchant.At = apikey.At;

        await db.SaveChangesAsync(cancellationToken);

        return new ApiKeyData {
            ApiKey = merchant.Id,
            ApiSecret = merchant.ApiSecret,
        };
    }
}
