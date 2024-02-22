using System.Linq.Dynamic.Core;
using DR.Common.Hashers;
using DR.Constant.Enums;
using DR.Helper;
using DR.Resource;

namespace DR.ManageApi.Application.Handlers.CustomerHandlers.Queries;

public class ListCustomerQuery : PaginatedRequest<ListCustomerData> {
    public int? DayBeforeLastPurchase { get; set; }
    public string? SortLabel { get; set; }
    public ESortDirection SortDirection { get; set; }
    public List<string>? ListProvinceCode { get; set; }
}

public class ListCustomerData : PaginatedList<CustomerDto> { }

internal class ListCustomerHandler(IServiceProvider serviceProvider) : BaseHandler<ListCustomerQuery, ListCustomerData>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<ListCustomerData> Handle(ListCustomerQuery request, CancellationToken cancellationToken) {
        var query = db.Customers.Include(o => o.CustomerGroup).AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
            .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
                var searchText = StringHelper.UnsignedUnicode(request.SearchText!);
                var searchPhone = PhoneHasher.Encrypt(searchText);
                return q.Where(o => o.Code.Contains(searchText!, StringComparison.CurrentCultureIgnoreCase)
                    || o.SearchName.Contains(searchText)
                    || o.CustomerPhones != null && o.CustomerPhones.Any(x => x.SearchPhone == searchPhone || x.SearchLastPhone == searchPhone));
            })
            .WhereFunc(request.ListProvinceCode != null && request.ListProvinceCode.Count > 0, q => {
                return q.Where(o => o.Province != null && request.ListProvinceCode!.Contains(o.Province));
            })
            .WhereFunc(request.DayBeforeLastPurchase.HasValue, q => {
                var currentMs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var requestMs = DateTimeHelper.MsInDay(request.DayBeforeLastPurchase);
                var queryMs = currentMs - requestMs;
                return q.Where(o => o.LastPurchase < queryMs);
            });

        int count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);

        CustomerDto? firstItem = null;
        if (string.IsNullOrWhiteSpace(request.SearchText) && !string.IsNullOrWhiteSpace(request.FirstItemId) && request.PageIndex == 0) {
            firstItem = await query.Where(o => o.Id == request.FirstItemId)
                .Select(o => CustomerDto.FromEntity(o, unitRes))
                .FirstOrDefaultAsync(cancellationToken);
            if (firstItem != null) {
                request.PageSize--;
                query = query.Where(o => o.Id != firstItem.Id);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.SortLabel) && request.SortDirection != ESortDirection.None) {
            var direction = request.SortDirection == ESortDirection.Descending ? "ASC" : "DESC";
            var sortRaw = $"{request.SortLabel} {direction}";
            query = query.OrderBy(sortRaw);
        } else {
            query = query.OrderBy(o => o.Code);
        }

        var items = await query.Skip(request.Skip).Take(request.Take)
            .Select(o => CustomerDto.FromEntity(o, unitRes))
            .ToListAsync(cancellationToken);

        if (firstItem != null) items.Insert(0, firstItem);

        return new() {
            Items = items,
            Count = count,
        };
    }
}
