using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DR.Database;
using MassTransit;
using Microsoft.AspNetCore.Http;
using DR.Common.Helpers;

namespace DR.Handlers {
    public abstract class BaseHandler {
        protected readonly IServiceProvider serviceProvider;
        protected readonly IMediator mediator;
        protected readonly IBus bus;
        protected readonly DrDbContext db;
        protected readonly IConfiguration configuration;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly string? url;

        protected BaseHandler(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
            this.bus = serviceProvider.GetRequiredService<IBusControl>();
            this.mediator = serviceProvider.GetRequiredService<IMediator>();
            this.db = serviceProvider.GetRequiredService<DrDbContext>();
            this.configuration = serviceProvider.GetRequiredService<IConfiguration>();
            this.httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            this.url = UrlHelper.GetCurrentUrl(httpContextAccessor.HttpContext?.Request, this.configuration, "ImageUrl");
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
}
