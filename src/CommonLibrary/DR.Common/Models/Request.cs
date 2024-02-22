using MediatR;

namespace DR.Common.Models;

public abstract class BaseRequest {
    public string MerchantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}

public abstract class Request : BaseRequest, IRequest {
}

public abstract class Request<TResponse> : BaseRequest, IRequest<TResponse> {
}

public abstract class BaseSingleRequest : BaseRequest {
    public string Id { get; set; } = string.Empty;
}

public class SingleRequest : BaseSingleRequest, IRequest {
}

public class SingleRequest<TResponse> : BaseSingleRequest, IRequest<TResponse> {
}

public abstract class BasePaginatedRequest : BaseRequest {
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public bool IsCount { get; set; }
    public string? SearchText { get; set; }
    public string? FirstItemId { get; set; }

    public int Skip => this.PageIndex * this.PageSize;
    public int Take => this.PageSize;
}

public class PaginatedRequest : BasePaginatedRequest, IRequest {
}

public class PaginatedRequest<TResponse> : BasePaginatedRequest, IRequest<TResponse> {
}

public abstract class BaseModelRequest<T> : BaseRequest
        where T : notnull {
    public T Model { get; set; } = default!;
}

public class ModelRequest<T> : BaseModelRequest<T>, IRequest
    where T : notnull {
}

public class ModelRequest<T, TResponse> : BaseModelRequest<T>, IRequest<TResponse>
    where T : notnull {
}
