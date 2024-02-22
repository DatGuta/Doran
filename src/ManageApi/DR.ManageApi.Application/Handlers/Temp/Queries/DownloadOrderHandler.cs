using System.Globalization;
using FluentFTP;
using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Constant.Enums;
using DR.Export;
using DR.Export.OrderExport;
using DR.Helper;
using DR.ManageApi.Application.Handlers;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;
using DR.Resource;

namespace TuanVu.Handlers.OrderHandlers;

public class DownloadOrderQuery : SingleRequest<FileResult> {
    public EExportFile ExportFile { get; set; }
    public bool IsHidePrice { get; set; }
}

internal class DownloadOrderHandler(IServiceProvider serviceProvider) : BaseHandler<DownloadOrderQuery, FileResult>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();
    private readonly ICultureInfoService cultureInfoService = serviceProvider.GetRequiredService<ICultureInfoService>();

    public override async Task<FileResult> Handle(DownloadOrderQuery request, CancellationToken cancellationToken) {
        var templateFile = await this.db.Files
            .Where(o => o.MerchantId == request.MerchantId && o.Type == EFile.Templates && o.ItemType == EItemType.Order)
            .FirstOrDefaultAsync(cancellationToken);
        ManagedException.ThrowIfNull(templateFile, Messages.Order.Export.Template_NotFound);

        var order = await this.db.Orders.Include(o => o.Store)
            .Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        ManagedException.ThrowIfNull(order, Messages.Order.Export.Order_NotFound);
        order.PrintNumber++;
        await this.db.SaveChangesAsync(cancellationToken);

        var items = await this.db.OrderDetails.AsNoTracking().Where(o => o.OrderId == order.Id).ToListAsync(cancellationToken);
        ManagedException.ThrowIf(items.Count == 0, Messages.Order.Export.Order_NotItems);

        CustomerDto? customer = null;
        var orderCustomer = await this.db.OrderCustomers.AsNoTracking()
            .Where(o => o.OrderId == order.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (orderCustomer != null) {
            customer = new CustomerDto {
                Name = orderCustomer.Name,
                Address = orderCustomer.Address,
                Phone = PhoneHasher.Decrypt(orderCustomer.Phone),
                Province = this.unitRes.GetByCode(orderCustomer.Province),
                District = this.unitRes.GetByCode(orderCustomer.District),
                Commune = this.unitRes.GetByCode(orderCustomer.Commune),
            };
        }

        var cultureInfo = await cultureInfoService.GetCultureInfo(request.MerchantId, cancellationToken);
        var data = new OrderExportData {
            day = order.CreatedDate.Day.ToString("00"),
            month = order.CreatedDate.Month.ToString("00"),
            year = order.CreatedDate.Year.ToString(),
            order_no = order.OrderNo,
            customer_name = customer?.Name ?? string.Empty,
            customer_address = customer?.GetFullAddress() ?? string.Empty,
            customer_phone = customer?.Phone ?? string.Empty,
            total_quantity = NumberHelper.N(items.Sum(o => o.Quantity), cultureInfo),
            total = request.IsHidePrice ? "-" : NumberHelper.C(order.TotalBill, cultureInfo),
            products = items.OrderBy(o => o.OrderIndex)
                .Select((o, idx) => new OrderExportDataItem {
                    no = (idx + 1).ToString(),
                    product_name = !string.IsNullOrEmpty(o.ProductDisplay) ? o.ProductDisplay : o.ProductName ?? string.Empty,
                    quantity = NumberHelper.N(o.Quantity, cultureInfo),
                    price = GetMoney(o.IsPromotion, request.IsHidePrice, o.Price, cultureInfo),
                    subtotal = GetMoney(o.IsPromotion, request.IsHidePrice, o.SubTotal, cultureInfo),
                }).ToList(),
        };

        string extension = Path.GetExtension(templateFile.Path);
        string localPath = $"templates/{NGuidHelper.New()}{extension}";
        string outputExtension = request.ExportFile switch {
            EExportFile.Excel => extension,
            EExportFile.Pdf => request.ExportFile.FileExtension(),
            _ => extension,
        };

        try {
            var status = await FtpHelper.DownloadFile(localPath, templateFile.Path, this.configuration);
            ManagedException.ThrowIf(status == FtpStatus.Failed, Messages.Order.Export.Template_NotFound);

            return new FileResult {
                FileName = $"{order.OrderNo}{outputExtension}",
                ByteArray = OrderExporter.Export(data, localPath, request.ExportFile),
            };
        } finally {
            if (File.Exists(localPath)) File.Delete(localPath);
        }
    }

    private static string GetMoney(bool isPromotion, bool isHidePrice, decimal value, CultureInfo cultureInfo) {
        return isPromotion ? "(KM)" : isHidePrice ? "-" : NumberHelper.C(value, cultureInfo);
    }
}
