using DR.Helper;

namespace DR.ManageApi.Application.Handlers.CategoryHandlers.Queries;

public class ListCategoryQuery : PaginatedRequest<ListCategoryData> { }

public class ListCategoryData : PaginatedList<CategoryDto> { }

public class ListCategoryHandler(IServiceProvider serviceProvider) : BaseHandler<ListCategoryQuery, ListCategoryData>(serviceProvider) {

    public override async Task<ListCategoryData> Handle(ListCategoryQuery request, CancellationToken cancellationToken) {
        var query = db.Categories.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
            .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
                var searchText = StringHelper.UnsignedUnicode(request.SearchText!);
                return q.Where(o => o.Code.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) || o.SearchName.Contains(searchText));
            });

        int count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);

        CategoryDto? firstItem = null;
        if (string.IsNullOrWhiteSpace(request.SearchText) && !string.IsNullOrWhiteSpace(request.FirstItemId) && request.PageIndex == 0) {
            firstItem = await query.Where(o => o.Id == request.FirstItemId)
                .Select(o => CategoryDto.FromEntity(o))
                .FirstOrDefaultAsync(cancellationToken);

            if (firstItem != null) {
                request.PageSize--;
                query = query.Where(o => o.Id != firstItem.Id);
            }
        }

        var items = await query.OrderBy(o => o.Code)
            .Skip(request.Skip).Take(request.Take)
            .Select(o => CategoryDto.FromEntity(o)).ToListAsync(cancellationToken);

        if (firstItem != null) items.Insert(0, firstItem);

        return new() {
            Items = items,
            Count = count
        };
    }
}
