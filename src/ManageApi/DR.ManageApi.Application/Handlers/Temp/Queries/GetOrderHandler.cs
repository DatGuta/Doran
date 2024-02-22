using DR.Constant.Enums;
using DR.ManageApi.Application.Handlers;
using DR.Resource;

namespace TuanVu.Handlers.OrderHandlers;

public class GetOrderQuery : SingleRequest<OrderDto?> {
    public bool IncludeExport { get; set; }
}

internal class GetOrderHandler(IServiceProvider serviceProvider) : BaseHandler<GetOrderQuery, OrderDto?>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<OrderDto?> Handle(GetOrderQuery request, CancellationToken cancellationToken) {
        var entity = await this.db.Orders.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null) return null;

        var order = OrderDto.FromEntity(entity, this.unitRes)!;

        order.Store = await this.db.Stores.AsNoTracking().Where(o => o.Id == entity.StoreId)
            .Select(o => StoreDto.FromEntity(o, this.unitRes)).FirstOrDefaultAsync(cancellationToken);

        order.Warehouse = await this.db.Warehouses.AsNoTracking().Where(o => o.Id == entity.WarehouseId)
            .Select(o => WarehouseDto.FromEntity(o, this.unitRes, false)).FirstOrDefaultAsync(cancellationToken);

        order.CreatedBy = await this.db.Users.AsNoTracking().Where(o => o.Id == entity.CreatedBy)
            .Select(o => UserDto.FromEntity(o, this.unitRes, null)).FirstOrDefaultAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(entity.CustomerId)) {
            order.Customer = await this.db.Customers.Include(o => o.CustomerGroup).AsNoTracking()
                .Where(o => o.Id == entity.CustomerId)
                .Select(o => CustomerDto.FromEntity(o, this.unitRes)).FirstOrDefaultAsync(cancellationToken);
        }

        order.Delivery = await this.db.OrderCustomers.AsNoTracking()
            .Where(o => o.OrderId == entity.Id)
            .Select(o => DeliveryDto.FromEntity(o, unitRes))
            .FirstOrDefaultAsync(cancellationToken);

        order.Items = await this.db.OrderDetails.Include(o => o.Product).AsNoTracking()
            .Where(o => o.OrderId == entity.Id)
            .OrderBy(o => o.OrderIndex)
            .Select(o => OrderDetailDto.FromEntity(o)!)
            .ToListAsync(cancellationToken);

        order.Actions = await this.GetActions(request.MerchantId, entity.Id, cancellationToken);

        order.ReceiptPayments = await this.GetReceiptPayments(request.MerchantId, entity.Id, cancellationToken);

        return order;
    }

    private async Task<List<OrderActionDto>> GetActions(string merchantId, string orderId, CancellationToken cancellationToken) {
        var actions = await this.db.OrderActions.AsNoTracking()
            .Where(o => o.OrderId == orderId && !o.IsDelete)
            .OrderBy(o => o.Time)
            .Select(o => OrderActionDto.FromEntity(o))
            .ToListAsync(cancellationToken);

        if (actions.Count > 0) {
            var actionIds = actions.Select(o => o.Id).ToList();
            var actionDetails = await this.db.OrderActionDetails.AsNoTracking()
                .Where(o => actionIds.Contains(o.OrderActionId))
                .ToListAsync(cancellationToken);

            var hasPayment = await this.db.PaymentDetails.AsNoTracking()
                .Where(o => o.OrderId == orderId && o.Payment!.MerchantId == merchantId && !o.Payment.IsDelete)
                .AnyAsync(cancellationToken);

            foreach (var action in actions) {
                if (action.Type == EOrderAction.Refund)
                    action.HasPayment = hasPayment;

                action.Items = actionDetails.Where(o => o.OrderActionId == action.Id)
                    .Select(o => OrderActionDetailDto.FromEntity(o))
                    .ToList();
            }
        }

        return actions;
    }

    private async Task<List<ReceiptPaymentDto>> GetReceiptPayments(string merchantId, string orderId, CancellationToken cancellationToken) {
        var receipts = this.db.Receipts.AsNoTracking()
            .Where(o => o.MerchantId == merchantId && o.ReceiptDetails!.Any(x => x.OrderId == orderId))
            .Select(o => new ReceiptPaymentDto {
                Id = o.Id,
                Type = EReceiptPayment.Receipt,
                Code = o.Code,
                TransactedAt = o.ReceiptDate,
                IsDelete = o.IsDelete,
            });

        var payments = this.db.Payments.AsNoTracking()
            .Where(o => o.MerchantId == merchantId && o.PaymentDetails!.Any(x => x.OrderId == orderId))
            .Select(o => new ReceiptPaymentDto {
                Id = o.Id,
                Type = EReceiptPayment.Payment,
                Code = o.Code,
                TransactedAt = o.PaymentDate,
                IsDelete = o.IsDelete,
            });

        return await receipts.Union(payments).OrderByDescending(o => o.TransactedAt).ToListAsync(cancellationToken);
    }
}
