using DR.Database;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace FMS.ManageApi.Application.Consumers;

public abstract class BaseRabbitMqConsumer(IServiceProvider serviceProvider) {
    protected readonly DrContext db = serviceProvider.GetRequiredService<DrContext>();
    protected readonly IBus bus = serviceProvider.GetRequiredService<IBusControl>();
    protected readonly IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
}

public abstract class BaseRabbitMqConsumer<TContext> : BaseRabbitMqConsumer, IConsumer<TContext>
    where TContext : class {

    protected BaseRabbitMqConsumer(IServiceProvider serviceProvider) : base(serviceProvider) {
    }

    public async Task Consume(ConsumeContext<TContext> context) {
        await Handle(context.Message, context.CancellationToken);
    }

    public abstract Task Handle(TContext context, CancellationToken cancellationToken);
}
