using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Lock;
using DR.Constant.Enums;
using DR.Contexts.AuditContexts;
using DR.Database.Models;
using DR.Helper;
using DR.ManageApi.Application.Handlers;
using DR.ManageApi.Domain.Actions.AutoGenerate;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;

namespace TuanVu.Handlers.OrderHandlers;

public class ExportOrderQuery : Request<string> {
    public string OrderId { get; set; } = string.Empty;
    public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
    public string? Code { get; set; }
    public int? Time { get; set; }
    public List<ExportOrderItem> Items { get; set; } = new();
}

public class ExportOrderItem {
    public string ItemId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

internal class ExportOrderHandler(IServiceProvider serviceProvider) : BaseHandler<ExportOrderQuery, string>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
    private readonly IOrderService orderService = serviceProvider.GetRequiredService<IOrderService>();
    private readonly IAutoGenerationService autoGenService = serviceProvider.GetRequiredService<IAutoGenerationService>();
    private readonly IProductTrackingService productTrackingService = serviceProvider.GetRequiredService<IProductTrackingService>();

    public override async Task<string> Handle(ExportOrderQuery request, CancellationToken cancellationToken) {
        request.Items ??= [];
        if (request.Items.Count == 0) return string.Empty;

        var order = await this.db.Orders.FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.OrderId, cancellationToken);
        ManagedException.ThrowIfNull(order, "Không tìm thấy đơn hàng này.");
        ManagedException.ThrowIfNull(order.WarehouseId, "Không tìm thấy kho này.");

        var handleStatus = new List<EOrderStatus> { EOrderStatus.New, EOrderStatus.Export, EOrderStatus.Exported };
        ManagedException.ThrowIfFalse(handleStatus.Contains(order.Status), $"Không thể xuất hàng với trạng thái đơn hàng là ${order.Status.Description()}");

        var ods = await this.db.OrderDetails.Where(o => o.OrderId == order.Id).ToListAsync(cancellationToken);
        var oas = await this.db.OrderActions.Include(o => o.OrderActionDetails)
            .Where(o => o.OrderId == order.Id && o.Type == EOrderAction.Export && !o.IsDelete)
            .ToListAsync(cancellationToken);

        request.Time ??= order.ExportTime + 1;

        OrderAction? original = null;

        var oa = oas.Find(o => o.Time == request.Time);
        if (oa == null) {
            await Locker.LockByKey($"Merchant:{request.MerchantId}:OrderExport");
            if (string.IsNullOrEmpty(request.Code)) {
                request.Code = await this.autoGenService.GenerateCode(request.MerchantId, ESetting.AutoGenerateWarehouseExportCode, EAutoGenerator.WarehouseExportCode, cancellationToken);
            } else {
                var existed = await this.db.Roles.AnyAsync(o => o.MerchantId == request.MerchantId && o.Code == request.Code, cancellationToken);
                ManagedException.ThrowIf(existed, Messages.WarehouseExport.CreateOrUpdate.WarehouseExport_Existed);
            }

            oa = new OrderAction {
                Id = NGuidHelper.New(),
                MerchantId = request.MerchantId,
                OrderId = order.Id,
                Type = EOrderAction.Export,
                Code = request.Code,
                Time = request.Time.Value,
                IsDelete = false,
            };
            await this.db.OrderActions.AddAsync(oa, cancellationToken);
        } else {
            original = oa.Clone();
        }
        oa.Date = request.Date;
        oa.OrderActionDetails ??= new List<OrderActionDetail>();

        var pts = new List<PtItem>();

        var oads = new List<OrderActionDetail>();
        foreach (var item in request.Items) {
            var od = ods.Find(o => o.Id == item.ItemId);
            if (od == null) continue;

            var oad = oa.OrderActionDetails.FirstOrDefault(o => o.OrderDetailId == item.ItemId);
            oad ??= new OrderActionDetail {
                Id = NGuidHelper.New(),
                OrderActionId = oa.Id,
                OrderDetailId = item.ItemId,
            };

            oad.Quantity = item.Quantity;
            oads.Add(oad);

            CreatePtItem(pts, od, oa, oad);
        }

        // remove oads
        var newOadIds = oads.Select(o => o.Id).ToList();
        var removeOads = oa.OrderActionDetails.Where(o => !newOadIds.Contains(o.Id)).ToList();
        foreach (var oad in removeOads) {
            var od = ods.Find(o => o.Id == oad.OrderDetailId);
            if (od == null) continue;

            CreatePtItem(pts, od, oa, oad, true);
        }

        oa.OrderActionDetails = oads;

        var otherOads = oas.Where(o => o.Id != oa.Id).SelectMany(o => o.OrderActionDetails!).ToList();
        foreach (var od in ods) {
            od.ExportQuantity = otherOads.Where(o => o.OrderDetailId == od.Id).Sum(o => o.Quantity)
                + oa.OrderActionDetails.Where(o => o.OrderDetailId == od.Id).Sum(o => o.Quantity);

            ManagedException.ThrowIf(od.ExportQuantity < od.RefundQuantity,
                "Không thể lưu phiếu xuất vì số lượng xuất nhỏ hơn số lượng trả hàng.");
        }

        if (order.ExportTime < request.Time)
            order.ExportTime = request.Time.Value;

        await this.db.SaveChangesAsync(cancellationToken);

        await this.orderService.Recalculate(order.Id, cancellationToken);
        await this.productTrackingService.Save(order.MerchantId, order.WarehouseId,
            EProductDocumentType.Export, order.Id, order.OrderNo, pts, cancellationToken);

        if (original == null) {
            await this.mediator.Publish(new OrderExpCAuditContext(request.MerchantId, request.UserId, order, oa), cancellationToken);
        } else {
            await this.mediator.Publish(new OrderExpUAuditContext(request.MerchantId, request.UserId, order, original, oa), cancellationToken);
        }

        return oa.Id;
    }

    private void CreatePtItem(List<PtItem> pts, OrderDetail od, OrderAction oa, OrderActionDetail oad, bool isDelete = false) {
        if (string.IsNullOrEmpty(od.ProductId))
            return;

        pts.Add(new PtItem {
            ProductId = od.ProductId,
            Value = !isDelete ? -oad.Quantity : 0,
            Time = oa.Date,
            TableName = $"{nameof(OrderActionDetail)}_{oa.Id}",
            ItemId = oad.Id,
            IsDelete = isDelete,
        });
    }
}
