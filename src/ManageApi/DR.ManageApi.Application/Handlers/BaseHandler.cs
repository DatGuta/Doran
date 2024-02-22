using DR.Database;
using Microsoft.Extensions.Configuration;

namespace DR.ManageApi.Application.Handlers;

public abstract class BaseHandler {
    protected readonly IConfiguration configuration;
    protected readonly DrContext db;
    protected readonly string? url;

    protected BaseHandler(IServiceProvider serviceProvider) {
        configuration = serviceProvider.GetRequiredService<IConfiguration>();
        db = serviceProvider.GetRequiredService<DrContext>();
        url = configuration["ImageUrl"];
    }
}

public abstract class BaseHandler<TRequest> : BaseHandler, IRequestHandler<TRequest>
        where TRequest : IRequest {

    protected BaseHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
    }

    public abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}

public abstract class BaseHandler<TRequest, TResponse> : BaseHandler, IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse> {

    protected BaseHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
    }

    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
