using DR.Export;
using DR.Export.RpCustomerHistory;
using DR.ManageApi.Domain.Services.Interfaces;

namespace DR.ManageApi.Application.Handlers.CustomerRpHandlers.Queries;

public class DownloadDebtHistoryQuery : Request<FileResult> {
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

internal class DownloadDebtHistory(IServiceProvider serviceProvider) : BaseHandler<DownloadDebtHistoryQuery, FileResult>(serviceProvider) {
    private readonly IGlobalSettingService globalSettingService = serviceProvider.GetRequiredService<IGlobalSettingService>();
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task<FileResult> Handle(DownloadDebtHistoryQuery request, CancellationToken cancellationToken) {
        var data = await mediator.Send(new ListDebtDetailQuery() {
            MerchantId = request.MerchantId,
            UserId = request.UserId,
            CustomerId = request.CustomerId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            IsAll = true
        }, cancellationToken);

        var setting = await globalSettingService.GetByMerchantId(request.MerchantId, cancellationToken);
        var customer = await db.Customers.AsNoTracking()
            .FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.CustomerId, cancellationToken);

        var no = 0;
        var report = new RpCustomerHistoryData() {
            From = request.FromDate!.Value.ToString("dd/MM/yyyy"),
            To = request.ToDate!.Value.ToString("dd/MM/yyyy"),
            CustomerName = customer?.Name ?? "",
            Items = data.Items.OrderBy(o => o.Date).Select(row => new RpCustomerHistoryItem {
                No = ++no,
                DocumentDate = row.Date.ToString("dd/MM/yyyy"),
                DocumentCode = row.Code,
                Reason = row.Reason ?? "",
                DebtBefore = row.DebtBefore,
                Debt = row.Debt,
                DebtAfter = row.DebtAfter,
                BalanceBefore = row.BalanceBefore,
                Balance = row.Balance,
                BalanceAfter = row.BalanceAfter
            }).ToList()
        };

        return new FileResult() {
            FileName = $"ChiTietCongNo_{DateTime.Now.ToLocalTime():yyyyMMdd_HHmmss}.xlsx",
            ByteArray = RpCustomerHistoryExporter.Export(report, EExportFile.Excel, setting.NumberFormat.GetCultureInfo().NumberFormat),
        };
    }
}
