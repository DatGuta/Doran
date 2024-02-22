using FMS.Constant.Enums;
using FMS.Contexts.NormalContexts;
using FMS.Database.Models;
using FMS.Helper;
using FMS.ManageApi.Domain.Services.Interfaces;
using MassTransit;

namespace FMS.ManageApi.Domain.Services.Implements;

public class CustomerTrackingService(IServiceProvider serviceProvider) : ICustomerTrackingService {
    private readonly FmsContext db = serviceProvider.GetRequiredService<FmsContext>();
    private readonly IBus bus = serviceProvider.GetRequiredService<IBusControl>();

    public async Task ProcessInit(string merchantId, string customerId, decimal value, CancellationToken cancellationToken) {
        await this.Process(merchantId, customerId,
            ECustomerDocType.Init, string.Empty, string.Empty,
            value, 0, DateTimeOffset.UnixEpoch, false,
            cancellationToken);
    }

    public async Task ProcessOrder(string orderId, CancellationToken cancellationToken) {
        var order = await this.db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
            return;

        if (string.IsNullOrWhiteSpace(order.CustomerId) || order.Status == EOrderStatus.Draft)
            return;

        await this.Process(order.MerchantId, order.CustomerId,
            ECustomerDocType.Order, order.Id, order.OrderNo,
            order.TotalBill, 0, order.CreatedDate, order.Status == EOrderStatus.Void,
            cancellationToken);
    }

    public async Task ProcessOrderRefund(string orderActionId, CancellationToken cancellationToken) {
        var orderAction = await this.db.OrderActions.AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderActionId && o.Type == EOrderAction.Refund, cancellationToken);
        if (orderAction == null) return;

        var order = await this.db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == orderAction.OrderId, cancellationToken);
        if (order == null || string.IsNullOrWhiteSpace(order.CustomerId)) return;

        var items = await this.db.OrderActionDetails.AsNoTracking()
            .Where(o => o.OrderActionId == orderAction.Id)
            .ToListAsync(cancellationToken);
        var debt = items.Sum(o => o.Quantity * o.Price);

        await this.Process(order.MerchantId, order.CustomerId,
            ECustomerDocType.OrderRefund, orderAction.Id, orderAction.Code,
            -debt, 0, orderAction.Date, orderAction.IsDelete,
            cancellationToken);
    }

    public async Task ProcessReceipt(string receiptId, CancellationToken cancellationToken) {
        var receipt = await this.db.Receipts.AsNoTracking().FirstOrDefaultAsync(o => o.Id == receiptId, cancellationToken);
        if (receipt == null) return;

        await this.Process(receipt.MerchantId, receipt.CustomerId,
            ECustomerDocType.Receipt, receipt.Id, receipt.Code,
            0, receipt.Value, receipt.ReceiptDate, receipt.IsDelete,
            cancellationToken);
    }

    public async Task ProcessPayment(string paymentId, CancellationToken cancellationToken) {
        var payment = await this.db.Payments.AsNoTracking().FirstOrDefaultAsync(o => o.Id == paymentId, cancellationToken);
        if (payment == null) return;

        if (payment.Type == EPayment.Standard) {
            await this.Process(payment.MerchantId, payment.CustomerId,
                ECustomerDocType.PaymentStandard, payment.Id, payment.Code,
                0, -payment.Value, payment.PaymentDate, payment.IsDelete,
                cancellationToken);
        }
    }

    private async Task Process(string merchantId, string customerId,
            ECustomerDocType docType, string docId, string docCode,
            decimal debt, decimal balance, DateTimeOffset date, bool isDelete,
            CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(merchantId)
            || string.IsNullOrWhiteSpace(customerId)) {
            return;
        }

        if (docType != ECustomerDocType.Init && string.IsNullOrWhiteSpace(docId)) {
            return;
        }

        var dt = await this.db.CustomerTrackings
            .Where(o => o.MerchantId == merchantId && o.CustomerId == customerId
                && o.DocumentType == docType && o.DocumentId == docId)
            .FirstOrDefaultAsync(cancellationToken);

        if (dt != null && dt.Date < date) {
            var nextDt = await this.db.CustomerTrackings
                .Where(o => o.MerchantId == merchantId && o.CustomerId == customerId && o.Date > dt.Date)
                .OrderBy(o => o.Date)
                .FirstOrDefaultAsync(cancellationToken);
            if (nextDt != null) {
                nextDt.IsUpdate = false;
            }
        }

        if (dt == null) {
            dt = new CustomerTracking {
                Id = NGuidHelper.New(),
                MerchantId = merchantId,
                CustomerId = customerId,
                DocumentType = docType,
                DocumentId = docId,
                DocumentCode = docCode,
            };
            await this.db.CustomerTrackings.AddAsync(dt, cancellationToken);
        }

        dt.IsUpdate = false;
        dt.IsDelete = isDelete;

        if (!dt.IsDelete) {
            dt.Debt = debt;
            dt.Balance = balance;
            dt.Date = date;
        }

        await this.db.SaveChangesAsync(cancellationToken);

        await this.bus.Publish(new UpdateCustomerTrackingContext(merchantId, customerId), cancellationToken);
    }
}
