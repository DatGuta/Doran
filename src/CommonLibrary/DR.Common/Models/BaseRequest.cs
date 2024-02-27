using MediatR;

namespace DR.Common.Models {
    public abstract class BaseRequest {
        public Guid MerchantId { get; set; } =Guid.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public abstract class BaseReq : BaseRequest, IRequest {
    }

    public abstract class BaseReq<TResponse> : BaseRequest, IRequest<TResponse> {
    }
}
