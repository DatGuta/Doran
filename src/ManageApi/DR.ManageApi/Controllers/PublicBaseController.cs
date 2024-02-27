using Crypto.AES;
using Newtonsoft.Json;
using DR.Common;

namespace DR.ManageApi.Controllers {
    public abstract class PublicBaseController : BaseController {
        protected readonly string merchantId;
        protected readonly string userId;

        protected PublicBaseController(IServiceProvider serviceProvider) : base(serviceProvider) {
            if (httpContext?.Request == null)
                throw new UnauthorizedAccessException("Invalid Api Key or Api Secret");

            if (!httpContext.Request.Headers.TryGetValue(Constants.HeaderApiKey, out var apiKeyStr))
                throw new UnauthorizedAccessException("Invalid Api Key or Api Secret");
            if (!httpContext.Request.Headers.TryGetValue(Constants.HeaderApiSecret, out var apiSecretStr))
                throw new UnauthorizedAccessException("Invalid Api Key or Api Secret");

            var apiInfo = JsonConvert.DeserializeObject<ApiKey>(AES.DecryptString(apiKeyStr.ToString(), apiSecretStr.ToString()))
                ?? throw new UnauthorizedAccessException("Invalid Api Key or Api Secret");

            this.merchantId = apiInfo.MerchantId;
            this.userId = apiInfo.UserId;
        }
    }
}
