using DR.Common;

namespace DR.ManageApi.Controllers {
    public abstract class PosBaseController : BaseController {
        protected readonly Guid merchantId;
        protected readonly string userId;
        protected readonly string storeId = string.Empty;
        protected readonly string warehouseId = string.Empty;

        protected PosBaseController(IServiceProvider serviceProvider) : base(serviceProvider) {
            var merchantIdClaim = httpContext?.User?.FindFirst(o => o.Type == Constants.TokenMerchantId)?.Value;
            this.merchantId = merchantIdClaim != null ? Guid.Parse(merchantIdClaim) : Guid.Empty;
            this.userId = httpContext?.User?.FindFirst(o => o.Type == Constants.TokenUserId)?.Value ?? string.Empty;

            if (httpContext?.Request != null && httpContext.Request.Headers.TryGetValue(Constants.HeaderStoreId, out var storeIdStr)) {
                this.storeId = storeIdStr.ToString();
            }

            if (httpContext?.Request != null && httpContext.Request.Headers.TryGetValue(Constants.HeaderWarehouseId, out var warehouseIdStr)) {
                this.warehouseId = warehouseIdStr.ToString();
            }
        }
    }
}
