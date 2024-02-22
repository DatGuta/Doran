using DR.Common.Exceptions;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.MerchantHandlers.Queries;

public class GetMerchantQuery : Request<MerchantDto> { }

internal class GetMerchantHandler(IServiceProvider serviceProvider) : BaseHandler<GetMerchantQuery, MerchantDto>(serviceProvider) {

    public override async Task<MerchantDto> Handle(GetMerchantQuery request, CancellationToken cancellationToken) {
        var merchant = await db.Merchants.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.MerchantId, cancellationToken);
        ManagedException.ThrowIf(merchant == null, Messages.Merchant.Get.Merchant_NotFound);
        ManagedException.ThrowIf(!merchant.IsActive, Messages.Merchant.Get.Merchant_Inactive);
        ManagedException.ThrowIf(merchant.ExpiredDate <= DateTimeOffset.UtcNow, Messages.Merchant.Get.Merchant_Expired);
        return MerchantDto.FromEntity(merchant);
    }
}
