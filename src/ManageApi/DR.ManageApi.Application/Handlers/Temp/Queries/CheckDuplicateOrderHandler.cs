using DR.Constant.Enums;
using DR.Database.Models;
using DR.Helper;
using DR.ManageApi.Application.Handlers;

namespace TuanVu.Handlers.OrderHandlers;

public class CheckDuplicateOrderQuery : ModelRequest<OrderDto, CheckDuplicateOrderRes> {
    public bool ShouldCheckDetail { get; set; } = false;
}

public class CheckDuplicateOrderRes {
    public bool IsDuplicate { get; set; } = false;
    public string? Id { get; set; }
    public string? OrderNo { get; set; }
    public List<OrderDetailDto>? Items { get; set; }
}

internal class CheckDuplicateOrderHandler(IServiceProvider serviceProvider)
    : BaseHandler<CheckDuplicateOrderQuery, CheckDuplicateOrderRes>(serviceProvider) {


    public override async Task<CheckDuplicateOrderRes> Handle(CheckDuplicateOrderQuery request, CancellationToken cancellationToken) {
        var model = request.Model;
        if (model == null) {
            return new CheckDuplicateOrderRes();
        }

        var merchantId = request.MerchantId;
        var warehouseId = model.Warehouse?.Id;
        var storeId = model.Store?.Id;
        var customerId = model.Customer?.Id;

        if (string.IsNullOrEmpty(warehouseId)
            || string.IsNullOrEmpty(storeId)
            || string.IsNullOrEmpty(customerId)
            || model.Items == null
            || model.Items.Count == 0) {
            return new CheckDuplicateOrderRes();
        }

        var (start, end) = DateTimeHelper.GetPeriod(DateTimeOffset.Now, EDateTimePeriod.Day);
        var handlerStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Export, EOrderStatus.Exported, EOrderStatus.Ticket };

        var existedOrders = await this.db.Orders.AsNoTracking()
            .Where(o => o.MerchantId == merchantId && o.WarehouseId == warehouseId && o.StoreId == storeId && o.CustomerId == customerId)
            .Where(o => start <= o.CreatedDate && o.CreatedDate <= end)
            .Where(o => handlerStatus.Contains(o.Status))
            .WhereIf(!string.IsNullOrWhiteSpace(model.Id), o => o.Id != model.Id)
            .OrderByDescending(o => o.CreatedDate)
            .Select(o => new { o.Id, o.OrderNo, o.CreatedDate }).ToListAsync(cancellationToken);
        if (existedOrders.Count > 0 && request.ShouldCheckDetail) {
            var orderIds = existedOrders.Select(o => o.Id).ToList();
            var items = await this.db.OrderDetails.AsNoTracking()
                .Where(o => orderIds.Contains(o.OrderId))
                .ToListAsync(cancellationToken);

            foreach (var existedOrder in existedOrders) {
                var orderItems = items.Where(o => o.OrderId == existedOrder.Id).ToList();
                if (IsDuplicate(model.Items, orderItems)) {
                    return new CheckDuplicateOrderRes {
                        IsDuplicate = true,
                        Id = existedOrder.Id,
                        OrderNo = existedOrder.OrderNo,
                        Items = items.Select(o => OrderDetailDto.FromEntity(o)).ToList(),
                    };
                }
            }
        } else if (existedOrders.Count > 0) {
            var existedOrder = existedOrders[0];
            return new CheckDuplicateOrderRes {
                IsDuplicate = true,
                Id = existedOrder.Id,
                OrderNo = existedOrder.OrderNo,
                Items = await this.db.OrderDetails.AsNoTracking()
                    .Where(o => o.OrderId == existedOrder.Id)
                    .Select(o => OrderDetailDto.FromEntity(o))
                    .ToListAsync(cancellationToken),
            };
        }

        return new CheckDuplicateOrderRes();
    }

    private static bool IsDuplicate(List<OrderDetailDto> dtos, List<OrderDetail> items) {
        if (dtos.Count != items.Count) return false;

        var modelCompareKey = dtos.Select(o => CompareKey(o.Product?.Id, o.Price, o.Quantity)).OrderBy(o => o).ToList();
        var orderCompareKey = items.Select(o => CompareKey(o.ProductId, o.Price, o.Quantity)).OrderBy(o => o).ToList();

        return modelCompareKey.SequenceEqual(orderCompareKey);
    }

    private static string CompareKey(string? productId, decimal price, decimal quantity) {
        return $"{productId}-{price:C2}-{quantity:N2}";
    }
}
