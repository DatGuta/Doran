using DR.Helper;

namespace DR.ManageApi.Application.Handlers.CustomerGroupHandlers.Queries;

public class ListCustomerGroupQuery : PaginatedRequest<ListCustomerGroupData> { }

public class ListCustomerGroupData : PaginatedList<CustomerGroupDto> { }

internal class ListCustomerGroupHandler(IServiceProvider serviceProvider) : BaseHandler<ListCustomerGroupQuery, ListCustomerGroupData>(serviceProvider) {

    public override async Task<ListCustomerGroupData> Handle(ListCustomerGroupQuery request, CancellationToken cancellationToken) {
        var query = db.CustomerGroups.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
            .WhereFunc(!string.IsNullOrEmpty(request.SearchText), q => {
                var searchText = StringHelper.UnsignedUnicode(request.SearchText);
                return q.Where(o => o.SearchName.Contains(searchText!));
            }).OrderBy(o => o.CreatedDate);

        var count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);
        var items = await query.Select(o => CustomerGroupDto.FromEntity(o)).ToListAsync(cancellationToken);

        return new ListCustomerGroupData {
            Count = count,
            Items = items
        };
    }
}
