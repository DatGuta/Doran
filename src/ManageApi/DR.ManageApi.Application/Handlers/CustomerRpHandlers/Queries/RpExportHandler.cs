using DR.Export;
using DR.Export.RpCustomer;
using DR.ManageApi.Domain.Services.Interfaces;

namespace DR.ManageApi.Application.Handlers.CustomerRpHandlers.Queries;

public class RpExportReq : Request<FileResult> {
    public string? SearchText { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool ShowAll { get; set; }
    public List<string> CustomerIds { get; set; } = new();
}

internal class RpExportHandler(IServiceProvider serviceProvider) : BaseHandler<RpExportReq, FileResult>(serviceProvider) {
    private readonly IGlobalSettingService globalSettingService = serviceProvider.GetRequiredService<IGlobalSettingService>();
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task<FileResult> Handle(RpExportReq request, CancellationToken cancellationToken) {
        var data = await mediator.Send(new ListCustomerRpQuery {
            MerchantId = request.MerchantId,
            SearchText = request.SearchText,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ShowAll = request.ShowAll,
            UserId = request.UserId,
            CustomerIds = request.CustomerIds,
            IsAll = true
        }, cancellationToken);

        var export = new RpCustomerExport {
            From = request.StartDate,
            To = request.EndDate,
        };
        var no = 1;
        foreach (var row in data.Items) {
            export.Items.Add(new RpCustomerItem {
                No = no++,
                Code = row.Code,
                Name = row.Name,
                OpeningDebtDisplay = row.OpeningDebt,
                CurrentDebtDisplay = row.CurrentDebt,
                OpeningBalanceDisplay = row.OpeningBalance,
                CurrentBalanceDisplay = row.CurrentBalance,
                EndDebtDisplay = row.EndDebt,
                EndBalanceDisplay = row.EndBalance
            });
        }

        var setting = await globalSettingService.GetByMerchantId(request.MerchantId, cancellationToken);

        return new FileResult {
            FileName = $"CongNoKhachHang_{DateTime.Now.ToLocalTime():yyyyMMdd_HHmmss}.xlsx",
            ByteArray = RpCustomerExporter.Export(export, EExportFile.Excel, setting.NumberFormat.GetCultureInfo().NumberFormat),
        };
    }
}

