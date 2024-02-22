namespace DR.ManageApi.Domain.Services.Interfaces;

public interface ICustomerTrackingService {

    Task ProcessInit(string merchantId, string customerId, decimal value, CancellationToken cancellationToken);

    Task ProcessOrder(string orderId, CancellationToken cancellationToken);

    Task ProcessOrderRefund(string orderActionId, CancellationToken cancellationToken);

    Task ProcessReceipt(string receiptId, CancellationToken cancellationToken);

    Task ProcessPayment(string paymentId, CancellationToken cancellationToken);
}
