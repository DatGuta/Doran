namespace DR.ManageApi.Domain.Services.Interfaces;

public interface IOrderService {

    Task Recalculate(string orderId, CancellationToken cancellationToken);

    Task Recalculate(List<string> orderIds, CancellationToken cancellationToken);
}
