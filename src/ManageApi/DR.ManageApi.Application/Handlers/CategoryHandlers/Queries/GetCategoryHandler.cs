namespace DR.ManageApi.Application.Handlers.CategoryHandlers.Queries;

public class GetCategoryQuery : SingleRequest<CategoryDto?> { }

internal class GetCategoryHandler(IServiceProvider serviceProvider) : BaseHandler<GetCategoryQuery, CategoryDto?>(serviceProvider) {

    public override async Task<CategoryDto?> Handle(GetCategoryQuery request, CancellationToken cancellationToken) {
        var category = await db.Categories.AsNoTracking().FirstOrDefaultAsync(
            o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete,
            cancellationToken
        );
        return CategoryDto.FromEntity(category);
    }
}
