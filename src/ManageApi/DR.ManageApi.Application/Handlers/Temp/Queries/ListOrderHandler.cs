using DR.Constant.Enums;
using DR.Database.Models;
using DR.Helper;
using DR.ManageApi.Application.Handlers;
using DR.Resource;

namespace TuanVu.Handlers.OrderHandlers;

public class ListOrderQuery : PaginatedRequest<ListOrderData> {
    public decimal? StartPrice { get; set; }
    public decimal? EndPrice { get; set; }
    public string? OrderNo { get; set; }
    public string? CreatedBy { get; set; }
    public string? CustomerId { get; set; }
    public string? StoreId { get; set; }
    public string? WarehouseId { get; set; }
    public List<EOrderStatus> Status { get; set; } = new();
    public List<EOrderPaymentStatus> PaymentStatus { get; set; } = new();
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool IsQuickViewUnpaid { get; set; }
    public bool HasCustomer { get; set; }
}

public class ListOrderData : PaginatedList<OrderDto> { }

internal class ListOrderHandler(IServiceProvider serviceProvider) : BaseHandler<ListOrderQuery, ListOrderData>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<ListOrderData> Handle(ListOrderQuery request, CancellationToken cancellationToken) {
        var handleOrderStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Export, EOrderStatus.Exported };
        var query = this.db.Orders.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId)
            .WhereFunc(!string.IsNullOrWhiteSpace(request.OrderNo), q => {
                var searchText = request.OrderNo!.Trim().ToUpper();
                return q.Where(o => o.OrderNo.Contains(searchText));
            })
            .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
                var searchText = request.SearchText!.Trim().ToUpper();
                var searchName = StringHelper.UnsignedUnicode(searchText);
                var queryCustomerIds = this.db.Customers.AsNoTracking()
                    .Where(o => o.MerchantId == request.MerchantId && o.SearchName.Contains(searchName))
                    .Select(o => o.Id);

                return q.Where(o => o.OrderNo.Contains(searchText)
                    || (!string.IsNullOrEmpty(o.CustomerId) && queryCustomerIds.Contains(o.CustomerId)));
            })
            .WhereIf(!string.IsNullOrWhiteSpace(request.CreatedBy), o => o.CreatedBy == request.CreatedBy)
            .WhereIf(request.HasCustomer, o => !string.IsNullOrWhiteSpace(o.CustomerId))
            .WhereIf(!string.IsNullOrWhiteSpace(request.CustomerId), o => o.CustomerId == request.CustomerId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.StoreId), o => o.StoreId == request.StoreId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.WarehouseId), o => o.WarehouseId == request.WarehouseId)
            .WhereIf(request.Status != null && request.Status.Any(), o => request.Status!.Contains(o.Status))
            .WhereIf(request.StartPrice.HasValue, o => o.TotalBill >= request.StartPrice)
            .WhereIf(request.EndPrice.HasValue, o => o.TotalBill <= request.EndPrice)
            .WhereFunc(request.IsQuickViewUnpaid,
                q => q.Where(o => o.PaymentStatus != EOrderPaymentStatus.Paid && handleOrderStatus.Contains(o.Status)),
                q => q.WhereIf(request.PaymentStatus != null && request.PaymentStatus.Any(), o => request.PaymentStatus!.Contains(o.PaymentStatus))
                    .WhereIf(request.StartDate.HasValue, o => request.StartDate <= o.CreatedDate)
                    .WhereIf(request.EndDate.HasValue, o => o.CreatedDate < request.EndDate));

        int count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);

        Order? firstItem = null;
        if (string.IsNullOrWhiteSpace(request.SearchText) && !string.IsNullOrWhiteSpace(request.FirstItemId) && request.PageIndex == 0) {
            firstItem = await query.Where(o => o.Id == request.FirstItemId).FirstOrDefaultAsync(cancellationToken);
            if (firstItem != null) {
                request.PageSize--;
                query = query.Where(o => o.Id != firstItem.Id);
            }
        }

        var queryOrder = request.IsQuickViewUnpaid
            ? query.OrderBy(o => o.CreatedDate)
            : query.OrderByDescending(o => o.CreatedDate);

        var items = await queryOrder.Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
        if (firstItem != null) items.Insert(0, firstItem);

        var customerIds = items.Where(o => !string.IsNullOrWhiteSpace(o.CustomerId)).Select(o => o.CustomerId).Distinct().ToList();
        var customers = await this.db.Customers.Include(o => o.CustomerGroup).AsNoTracking()
            .Where(o => customerIds.Contains(o.Id)).ToDictionaryAsync(o => o.Id, cancellationToken);

        var orderIds = items.Where(o => string.IsNullOrWhiteSpace(o.CustomerId)).Select(o => o.Id).ToList();
        var orderCustomers = new Dictionary<string, OrderCustomer>();
        if (orderIds.Count > 0) {
            orderCustomers = await this.db.OrderCustomers.AsNoTracking().Where(o => orderIds.Contains(o.OrderId)).ToDictionaryAsync(o => o.OrderId, cancellationToken);
        }

        var orders = new List<OrderDto>();
        foreach (var item in items) {
            var order = OrderDto.FromEntity(item, this.unitRes);
            if (!string.IsNullOrEmpty(item.CustomerId) && customers.TryGetValue(item.CustomerId, out var customer))
                order.Customer = CustomerDto.FromEntity(customer, this.unitRes);

            if (orderCustomers.TryGetValue(item.Id, out var orderCustomer))
                order.Delivery = DeliveryDto.FromEntity(orderCustomer, this.unitRes);

            orders.Add(order);
        }

        return new() {
            Items = orders,
            Count = count,
        };
    }
}
