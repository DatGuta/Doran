using DR.Constant.Enums;
using DR.Database.Models;
using DR.Helper;
using Quartz;

namespace DR.ManageApi.Application.Jobs;

[DisallowConcurrentExecution]
public class CalculatingSaleOrderJob(IServiceProvider serviceProvider) : IJob {
    private readonly FmsContext db = serviceProvider.GetRequiredService<FmsContext>();

    public async Task Execute(IJobExecutionContext context) {
        var now = DateTimeOffset.Now;
        var merchantIds = await this.db.Merchants.AsNoTracking()
            .Where(o => o.IsActive && o.ExpiredDate > now)
            .Select(o => o.Id).ToListAsync();

        var merchantMinDates = await this.db.ReportSales.AsNoTracking()
            .Where(o => merchantIds.Contains(o.MerchantId))
            .GroupBy(o => o.MerchantId)
            .Select(o => new { o.Key, MinDate = o.Max(x => x.Date) })
            .ToDictionaryAsync(k => k.Key, v => v.MinDate.LocalDateTime);

        List<ReportSale> reports = [];
        var minDate = DateTimeOffset.MinValue.LocalDateTime;

        foreach (var merchantId in merchantIds) {
            DateTimeOffset endDate = now.LocalDateTime;
            DateTimeOffset startDate = merchantMinDates.GetValueOrDefault(merchantId, minDate);

            if (startDate.Date.CompareTo(minDate.Date) <= 0) {
                var firstOrder = await this.db.Orders.AsNoTracking()
                    .Where(o => o.MerchantId == merchantId && o.Status != EOrderStatus.Void)
                    .OrderBy(o => o.CreatedDate)
                    .FirstOrDefaultAsync();
                if (firstOrder == null) continue;

                startDate = firstOrder.CreatedDate.LocalDateTime;
            } else {
                startDate = startDate.AddDays(1);
            }
            if (startDate.Date.CompareTo(now.Date) >= 0) continue;

            var merchantReports = await ProgressDate(merchantId, startDate, endDate);
            if (merchantReports != null && merchantReports.Count != 0)
                reports = [.. reports, .. merchantReports];
        }

        var insertDatas = reports.Chunk(1000).ToList();
        foreach (var insertData in insertDatas) {
            await this.db.ReportSales.AddRangeAsync(insertData);
            await this.db.SaveChangesAsync();
        }
    }

    private async Task<List<ReportSale>?> ProgressDate(string merchantId, DateTimeOffset startDate, DateTimeOffset endDate) {
        endDate = new DateTimeOffset(endDate.Date);
        var orderDates = await this.db.Orders.AsNoTracking().Where(o => o.MerchantId == merchantId
               && startDate < o.CreatedDate && o.CreatedDate < endDate && o.Status != EOrderStatus.Void)
           .ToListAsync();

        if (orderDates == null || orderDates.Count == 0) return null;

        var reportSales = orderDates.GroupBy(o => o.CreatedDate.LocalDateTime.Date)
            .Select(o => new ReportSale {
                Id = NGuidHelper.New(),
                MerchantId = merchantId,
                Date = o.Key,
                CountOrder = o.Count(),
                Total = o.Sum(od => od.TotalBill)
            }).ToList();

        return reportSales;
    }
}
