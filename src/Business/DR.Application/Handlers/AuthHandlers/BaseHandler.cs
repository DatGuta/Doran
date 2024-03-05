namespace DR.Application.Handlers.AuthHandlers;

public abstract class BaseHandler(IServiceProvider serviceProvider) : BaseMediatR(serviceProvider) {
    protected readonly string? url = serviceProvider.GetRequiredService<IConfiguration>()["ImageUrl"];
}

public abstract class BaseHandler<TRequest>(IServiceProvider serviceProvider)
    : BaseHandler(serviceProvider), IRequestHandler<TRequest>
        where TRequest : IRequest {

    public abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}

public abstract class BaseHandler<TRequest, TResponse>(IServiceProvider serviceProvider)
    : BaseHandler(serviceProvider), IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
