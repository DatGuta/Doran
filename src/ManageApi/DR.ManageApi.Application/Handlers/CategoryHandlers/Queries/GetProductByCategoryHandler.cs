namespace DR.ManageApi.Application.Handlers.CategoryHandlers.Queries;

public class GetProductByCategoryQuery : SingleRequest<List<ProductOnCategoryDto>> { }

internal class GetProductByCategoryHandler(IServiceProvider serviceProvider)
    : BaseHandler<GetProductByCategoryQuery, List<ProductOnCategoryDto>>(serviceProvider) {

    public override async Task<List<ProductOnCategoryDto>> Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken) {
        var productIds = await db.ProductCategories.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.CategoryId == request.Id && !o.IsDelete)
            .Select(o => o.ProductId).ToListAsync(cancellationToken);

        var result = await db.Products.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
            .Select(o => new ProductOnCategoryDto() {
                Id = o.Id,
                Code = o.Code,
                Name = o.Name,
                IsPromotion = o.IsPromotion,
                IsOnCategory = productIds.Contains(o.Id),
            }).ToListAsync(cancellationToken);

        return result;
    }
}
