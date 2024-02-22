using Crypto.AES;
using Newtonsoft.Json;

namespace DR.Helper;

public class ApiKey {
    public string MerchantId { get; init; }
    public string UserId { get; init; }
    public long At { get; init; }

    public ApiKey(string merchantId, string systemUserId) {
        this.MerchantId = merchantId;
        this.UserId = systemUserId;
        this.At = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public string GetSecret()
        => AES.EncryptString(this.MerchantId, JsonConvert.SerializeObject(this, Formatting.None));
}
