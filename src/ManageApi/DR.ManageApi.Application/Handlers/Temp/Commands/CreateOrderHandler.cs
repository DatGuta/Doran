using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Common.Lock;
using DR.Constant;
using DR.Constant.Enums;
using DR.Contexts.AuditContexts;
using DR.Contexts.NormalContexts;
using DR.Database.Models;
using DR.Helper;
using DR.ManageApi.Application.Handlers;
using DR.ManageApi.Domain.Actions.AutoGenerate;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;
using MassTransit;

namespace TuanVu.Handlers.OrderHandlers;

public class CreateOrderCommand : ModelRequest<OrderDto, CreateOrderRes> {
    public ESource Source { get; set; } = ESource.WEB;
}

public class CreateOrderRes {
    public string OrderId { get; set; } = null!;
    public string OrderNo { get; set; } = null!;
}

internal class CreateOrderHandler(IServiceProvider serviceProvider) : BaseHandler<CreateOrderCommand, CreateOrderRes>(serviceProvider) {
    private readonly IBus bus = serviceProvider.GetRequiredService<IBus>();
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
    private readonly IOrderService orderService = serviceProvider.GetRequiredService<IOrderService>();
    private readonly ISettingService settingService = serviceProvider.GetRequiredService<ISettingService>();
    private readonly IAutoGenerationService autoGenerationService = serviceProvider.GetRequiredService<IAutoGenerationService>();
    private readonly ICustomerTrackingService customerTrackingService = serviceProvider.GetRequiredService<ICustomerTrackingService>();

    public override async Task<CreateOrderRes> Handle(CreateOrderCommand request, CancellationToken cancellationToken) {
        var model = request.Model;

        model.Id = NGuidHelper.New();
        ManagedException.ThrowIfNullOrEmpty(model.Items, Messages.Order.Create.Order_NoItems);

        Warehouse? warehouse = null;
        if (!string.IsNullOrEmpty(model.Warehouse?.Id)) {
            warehouse = await this.db.Warehouses.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && o.Id == model.Warehouse.Id! && !o.IsDelete && o.IsActive)
                .FirstOrDefaultAsync(cancellationToken);
            ManagedException.ThrowIfNull(warehouse, Messages.Order.Create.Warehouse_NotFound);
        }

        var order = await Locker.LockByKey($"Merchant:{request.MerchantId}:Order",
            () => this.CreateOrder(request.MerchantId, request.UserId, request.Source, model, warehouse, cancellationToken));

        if (order == null) return new CreateOrderRes();

        if (!string.IsNullOrWhiteSpace(order.WarehouseId)) {
            var allowAutoExportOrder = await this.settingService.GetValue(order.MerchantId, ESetting.AutoExportOrder, false, cancellationToken);
            if (allowAutoExportOrder) {
                await this.mediator.Send(new ExportOrderQuery {
                    MerchantId = request.MerchantId,
                    UserId = request.UserId,
                    OrderId = order.Id,
                    Date = order.CreatedDate,
                    Items = order.OrderDetails!.Select(o => new ExportOrderItem {
                        ItemId = o.Id,
                        Quantity = o.Quantity,
                    }).ToList(),
                }, cancellationToken);
            }
        }

        await this.customerTrackingService.ProcessOrder(order.Id, cancellationToken);

        return new CreateOrderRes { OrderId = order.Id, OrderNo = order.OrderNo };
    }

    private async Task<Order> CreateOrder(string merchantId, string userId, ESource source, OrderDto model, Warehouse? warehouse, CancellationToken cancellationToken) {
        EOrderStatus orderType;
        if (warehouse == null) {
            orderType = EOrderStatus.Draft;
        } else {
            orderType = warehouse.Type == EWarehouse.Normal ? EOrderStatus.New : EOrderStatus.Ticket;
        }

        var productIds = model.Items?.Where(o => !string.IsNullOrWhiteSpace(o.Product?.Id))
                .Select(o => o.Product!.Id!).Distinct().ToList() ?? new List<string>();
        var products = await this.db.Products.AsNoTracking()
            .Where(o => o.MerchantId == merchantId && productIds.Contains(o.Id))
            .ToDictionaryAsync(k => k.Id, v => new { v.Code, v.Name, v.DisplayName }, cancellationToken);

        var order = new Order {
            Id = NGuidHelper.New(model.Id),
            MerchantId = merchantId,
            StoreId = model.Store!.Id!,
            WarehouseId = warehouse?.Id,
            Type = Constants.OrderTypeMap.GetValueOrDefault(warehouse?.Type ?? EWarehouse.Normal, EOrder.Normal),
            OrderNo = await this.autoGenerationService.GenerateCode(merchantId, ESetting.AutoGenerateOrderNo, EAutoGenerator.OrderNo, cancellationToken),
            Status = orderType,
            CustomerId = !string.IsNullOrEmpty(model.Customer?.Id) ? model.Customer.Id : default,
            DiscountId = model.Discount?.Id,
            DiscountType = model.Discount?.Type ?? EDiscount.None,
            DiscountValue = model.Discount?.Value ?? default,
            SubTotal = model.SubTotal,
            BillDiscount = model.BillDiscount,
            SubDiscount = model.SubDiscount,
            TotalDiscount = model.TotalDiscount,
            TotalBill = model.TotalBill,
            PaymentStatus = EOrderPaymentStatus.Unpaid,
            Remaining = model.TotalBill,
            Paid = 0,
            IsUseCustomerInfo = model.IsUseCustomerInfo,
            Description = model.Description,
            CreatedBy = model.CreatedBy?.Id ?? userId,
            OrderDetails = model.Items!.Select(o => {
                o.Id = NGuidHelper.New(o.Id);
                var item = new OrderDetail {
                    Id = o.Id,
                    GroupId = o.Id,
                    ProductId = o.Product?.Id,
                    OrderIndex = o.OrderIndex,
                    ProductCode = o.Product?.Code,
                    ProductName = o.Product?.Name,
                    ProductDisplay = o.Product?.DisplayName,
                    IsPromotion = o.IsPromotion,
                    DiscountId = o.Discount?.Id,
                    DiscountType = o.Discount?.Type ?? EDiscount.None,
                    DiscountValue = o.Discount?.Value ?? default,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    SubTotal = o.SubTotal,
                    ItemDiscount = o.ItemDiscount,
                    TotalItem = o.TotalItem,
                };

                if (!string.IsNullOrWhiteSpace(o.Product?.Id)
                    && products.TryGetValue(o.Product.Id!, out var product)) {
                    item.ProductCode = product.Code;
                    item.ProductName = product.Name;
                    item.ProductDisplay = product.DisplayName;
                }

                return item;
            }).ToList(),
            OrderCustomers = GetDelivery(model.Delivery),
        };

        await this.db.Orders.AddAsync(order, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);

        await this.orderService.Recalculate(order.Id, cancellationToken);
        await this.mediator.Publish(new OrderCAuditContext(merchantId, userId, order, source), cancellationToken);

        if (!string.IsNullOrWhiteSpace(order.CustomerId)) {
            await this.bus.Publish(new CustomerLastPurchaseContext(merchantId, order.CustomerId), cancellationToken);
        }

        return order;
    }

    private static List<OrderCustomer> GetDelivery(DeliveryDto? dto) {
        var orderCustomers = new List<OrderCustomer>();
        if (dto != null) {
            orderCustomers.Add(new OrderCustomer() {
                Id = NGuidHelper.New(),
                Name = dto.Name,
                Phone = PhoneHasher.Encrypt(dto.Phone) ?? string.Empty,
                Province = dto.Province?.Code,
                District = dto.District?.Code,
                Commune = dto.Commune?.Code,
                Address = dto.Address,
            });
        }
        return orderCustomers;
    }
}

