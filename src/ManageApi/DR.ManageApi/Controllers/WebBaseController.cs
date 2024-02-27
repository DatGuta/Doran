using DR.Common;
using DR.ManageApi.Controllers;

namespace DR.ManageApi.Controllers {
    public abstract class WebBaseController : BaseController {
        protected readonly Guid merchantId;
        protected readonly string userId;

        protected WebBaseController(IServiceProvider serviceProvider) : base(serviceProvider) {
            var merchantIdClaim = httpContext?.User?.FindFirst(o => o.Type == Constants.TokenMerchantId)?.Value;
            this.merchantId = merchantIdClaim != null ? Guid.Parse(merchantIdClaim) : Guid.Empty;
            this.userId = httpContext?.User?.FindFirst(o => o.Type == Constants.TokenUserId)?.Value ?? string.Empty;
        }
    }
}
