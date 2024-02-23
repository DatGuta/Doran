
using Microsoft.Extensions.Configuration;

namespace DR.ManageApi.Application.Consumers;

public abstract class BaseConsumer(IServiceProvider serviceProvider) {
    // protected readonly DrContext db = serviceProvider.GetRequiredService<DrContext>();
    protected readonly IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
}

public abstract class BaseConsumer<TContext> : BaseConsumer, INotificationHandler<TContext>
    where TContext : INotification {

    protected BaseConsumer(IServiceProvider serviceProvider) : base(serviceProvider) {
    }

    public abstract Task Handle(TContext context, CancellationToken cancellationToken);
}
