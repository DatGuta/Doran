using DR.Helper;

namespace DR.ManageApi.Application.Handlers.CustomerHandlers.Queries;

public class SearchCustomerQuery : Request<List<SearchCustomerDto>> {
    public string? SearchText { get; set; }
    public List<string> CustomerGroupIds { get; set; } = [];
}

internal class SearchCustomerHandler(IServiceProvider serviceProvider) : BaseHandler<SearchCustomerQuery, List<SearchCustomerDto>>(serviceProvider) {

    public override async Task<List<SearchCustomerDto>> Handle(SearchCustomerQuery request, CancellationToken cancellationToken) {
        var query = db.Customers.AsNoTracking().Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
            .WhereIf(request.CustomerGroupIds != null && request.CustomerGroupIds.Count > 0,
                o => !string.IsNullOrEmpty(o.CustomerGroupId) && request.CustomerGroupIds!.Contains(o.CustomerGroupId))
            .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
                var searchText = StringHelper.UnsignedUnicode(request.SearchText!);
                return q.Where(o => o.Code.Contains(searchText.ToUpper()) || o.SearchName.ToLower().Contains(searchText));
            });

        return await query.OrderBy(o => o.Code).Select(o => SearchCustomerDto.FromEntity(o)).ToListAsync(cancellationToken);
    }
}
