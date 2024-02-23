using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.Temp.Queries;

public class CreateOrderCommand : ModelRequest<OrderDto, CreateOrderRes> {
    public ESource Source { get; set; } = ESource.WEB;
}

public class CreateOrderRes {
    public string OrderId { get; set; } = null!;
    public string OrderNo { get; set; } = null!;
}

internal class CreateOrderHandler(IServiceProvider serviceProvider) : BaseHandler<CreateOrderCommand, CreateOrderRes>(serviceProvider) {
    //private readonly IBus bus = serviceProvider.GetRequiredService<IBus>();
    //private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
    //private readonly IOrderService orderService = serviceProvider.GetRequiredService<IOrderService>();
    //private readonly ISettingService settingService = serviceProvider.GetRequiredService<ISettingService>();
    //private readonly IAutoGenerationService autoGenerationService = serviceProvider.GetRequiredService<IAutoGenerationService>();
    //private readonly ICustomerTrackingService customerTrackingService = serviceProvider.GetRequiredService<ICustomerTrackingService>();

    public override Task<CreateOrderRes> Handle(CreateOrderCommand request, CancellationToken cancellationToken) {
        throw new Exception("Not implemented");
    }
}
