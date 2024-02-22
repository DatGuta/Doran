namespace DR.ManageApi.Application.Handlers.CategoryHandlers.Queries;

public class AllCategoryQuery : PaginatedRequest<ListCategoryData> { }

internal class AllCategoryHandler(IServiceProvider serviceProvider) : BaseHandler<ListCategoryQuery, ListCategoryData>(serviceProvider) {

    public override async Task<ListCategoryData> Handle(ListCategoryQuery request, CancellationToken cancellationToken) {
        var items = await db.Categories.AsNoTracking()
           .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
           .OrderBy(o => o.Code).Select(o => CategoryDto.FromEntity(o))
           .ToListAsync(cancellationToken);

        return new() { Items = items, Count = items.Count };
    }
}
