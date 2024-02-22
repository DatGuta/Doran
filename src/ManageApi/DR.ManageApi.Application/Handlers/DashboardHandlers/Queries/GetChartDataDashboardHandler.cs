namespace DR.ManageApi.Application.Handlers.DashboardHandlers.Queries;

public class GetChartDataQuery : Request<List<DashboardCharDto>> { }

internal class GetChartDataDashboardHandler(IServiceProvider serviceProvider) : BaseHandler<GetChartDataQuery, List<DashboardCharDto>>(serviceProvider) {

    public override async Task<List<DashboardCharDto>> Handle(GetChartDataQuery request, CancellationToken cancellationToken) {
        var to = DateTimeOffset.Now.AddDays(1).Date;
        var from = to.AddDays(-31).Date;
        var data = await db.ReportSales.AsNoTracking()
            .Where(o => from <= o.Date && o.Date <= to && o.MerchantId == request.MerchantId)
            .ToDictionaryAsync(o => o.Date, cancellationToken);

        var result = new List<DashboardCharDto>();
        for (DateTimeOffset date = from; date < to; date = date.AddDays(1)) {
            var orderOfDay = data.GetValueOrDefault(date.Date);
            result.Add(new DashboardCharDto {
                Date = date,
                Count = orderOfDay?.CountOrder ?? 0,
                Total = (orderOfDay?.Total ?? 0) / 1000000M
            });
        }
        return result;
    }
}
