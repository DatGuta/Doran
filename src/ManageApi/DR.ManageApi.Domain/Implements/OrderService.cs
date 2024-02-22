using DR.Constant.Enums;
using DR.Database.Models;
using DR.ManageApi.Domain.Services.Interfaces;

namespace DR.ManageApi.Domain.Services.Implements;

public class OrderService(IServiceProvider serviceProvider) : IOrderService {
    private readonly DrContext db = serviceProvider.GetRequiredService<DrContext>();

    public async Task Recalculate(string orderId, CancellationToken cancellationToken) {
        await this.Recalculate(new List<string> { orderId }, cancellationToken);
    }

    public async Task Recalculate(List<string> orderIds, CancellationToken cancellationToken) {
        if (orderIds.Count == 0) return;

        var orders = await this.db.Orders.Where(o => orderIds.Contains(o.Id)).ToListAsync(cancellationToken);
        if (orders.Count == 0) return;

        var items = await this.db.OrderDetails.Where(o => orderIds.Contains(o.OrderId)).ToListAsync(cancellationToken);

        var actions = await this.db.OrderActionDetails.AsNoTracking()
            .Where(o => orderIds.Contains(o.OrderAction!.OrderId) && !o.OrderAction.IsDelete)
            .Select(o => new {
                o.OrderAction!.Type,
                o.OrderDetailId,
                o.Price,
                o.Quantity,
            }).ToListAsync(cancellationToken);

        foreach (var item in items) {
            if (item.IsPromotion) {
                item.SubTotal = 0;
                item.ItemDiscount = 0;
                item.TotalItem = 0;
            } else {
                item.SubTotal = item.Price * item.Quantity;
                item.ItemDiscount = CalculateDiscount(item.SubTotal, item.DiscountType, item.DiscountValue);
                item.TotalItem = item.SubTotal - item.ItemDiscount;
            }

            var exportItems = actions.FindAll(o => o.OrderDetailId == item.Id && o.Type == EOrderAction.Export);
            item.ExportQuantity = exportItems.Sum(o => o.Quantity);

            var refundItems = actions.FindAll(o => o.OrderDetailId == item.Id && o.Type == EOrderAction.Refund);
            item.RefundQuantity = refundItems.Sum(o => o.Quantity);
            item.RefundAmount = refundItems.Sum(o => o.Quantity * o.Price);
        }

        var receipts = await this.db.ReceiptDetails.AsNoTracking()
            .Where(o => orderIds.Contains(o.OrderId) && !o.Receipt!.IsDelete)
            .GroupBy(o => o.OrderId)
            .ToDictionaryAsync(k => k.Key, v => v.Sum(o => o.Value), cancellationToken);

        var payments = await this.db.PaymentDetails.AsNoTracking()
            .Where(o => orderIds.Contains(o.OrderId!) && !o.Payment!.IsDelete)
            .GroupBy(o => o.OrderId!)
            .ToDictionaryAsync(k => k.Key, v => v.Sum(o => o.Value), cancellationToken);

        foreach (var order in orders) {
            var orderItems = items.FindAll(o => o.OrderId == order.Id);

            order.TotalOverExport = orderItems.Sum(o => {
                var diffQuantity = o.ExportQuantity - o.Quantity;
                if (diffQuantity > 0)
                    return diffQuantity * o.Price;
                return 0;
            });

            order.SubTotal = orderItems.Sum(o => o.SubTotal);
            order.SubDiscount = orderItems.Sum(o => o.ItemDiscount);
            order.BillDiscount = CalculateDiscount(order.SubTotal, order.DiscountType, order.DiscountValue);
            order.TotalDiscount = order.SubDiscount + order.BillDiscount;
            order.TotalBill = order.SubTotal - order.TotalDiscount;

            order.Paid = receipts.GetValueOrDefault(order.Id);
            order.Refunded = payments.GetValueOrDefault(order.Id);
            order.TotalRefundQuantity = orderItems.Sum(o => o.RefundQuantity);
            order.TotalRefund = orderItems.Sum(o => o.RefundAmount);

            order.Remaining = order.TotalBill - order.Paid - order.TotalRefund + order.Refunded;
            order.PaymentStatus = GetOrderPaymentStatus(order, receipts.ContainsKey(order.Id));
            order.Status = GetOrderStatus(order, orderItems);
        }

        await this.db.SaveChangesAsync(cancellationToken);
    }

    private decimal CalculateDiscount(decimal subtotal, EDiscount discountType, decimal discountValue) {
        if (subtotal == 0 || discountType == EDiscount.None)
            return 0;
        if (discountType == EDiscount.Value)
            return discountValue;
        if (discountType == EDiscount.Percent)
            return subtotal * discountValue / 100M;
        return 0;
    }

    private EOrderStatus GetOrderStatus(Order order, List<OrderDetail> items) {
        if (order.Status == EOrderStatus.Draft)
            return EOrderStatus.Draft;

        if (items.Exists(o => o.ExportQuantity > 0)) {
            if (items.Exists(o => o.ExportQuantity < o.Quantity)) {
                return EOrderStatus.Export;
            }
            return EOrderStatus.Exported;
        }
        return EOrderStatus.New;
    }

    private EOrderPaymentStatus GetOrderPaymentStatus(Order order, bool hasReceipt) {
        if (order.Remaining <= 0)
            return hasReceipt ? EOrderPaymentStatus.Paid : EOrderPaymentStatus.Unpaid;
        if (order.Paid == 0)
            return EOrderPaymentStatus.Unpaid;
        return EOrderPaymentStatus.PartialPaid;
    }
}
