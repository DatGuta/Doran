using DR.Contexts.AuditContexts;
using DR.Database.Models;
using DR.Helper;

namespace DR.ManageApi.Application.Handlers.CategoryHandlers.Commands;

public class SaveProductByCategoryCommand : Request {
    public bool IsAdd { get; set; }
    public List<string> ProductIds { get; set; } = [];
    public string? CategoryId { get; set; }
}

internal class SaveProductByCategoryHandler(IServiceProvider serviceProvider) : BaseHandler<SaveProductByCategoryCommand>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
    public override async Task Handle(SaveProductByCategoryCommand request, CancellationToken cancellationToken) {
        if (string.IsNullOrWhiteSpace(request.CategoryId)) return;
        if (request.ProductIds == null || request.ProductIds.Count == 0) return;

        var category = await db.Categories.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.CategoryId, cancellationToken);
        if (category == null) return;

        var items = await db.ProductCategories.Where(
            o => o.MerchantId == request.MerchantId && o.CategoryId == category.Id
        ).ToListAsync(cancellationToken);

        var productIds = items.Select(o => o.ProductId).ToList();
        var addProductIds = new List<string>();
        var deleteProductIds = new List<string>();

        foreach (var item in items) {
            if (item.IsDelete && request.ProductIds.Contains(item.ProductId)) {
                item.IsDelete = false;
                item.ModifiedDate = DateTimeOffset.UtcNow;
                addProductIds.Add(item.ProductId);
                continue;
            }

            if (!item.IsDelete && !request.ProductIds.Contains(item.ProductId)) {
                item.IsDelete = true;
                item.ModifiedDate = DateTimeOffset.UtcNow;
                deleteProductIds.Add(item.ProductId);
                continue;
            }
        }

        var insertProductOnCategory = request.ProductIds.Where(o => !productIds.Contains(o)).Select(o => new ProductCategory() {
            Id = NGuidHelper.New(),
            MerchantId = request.MerchantId,
            CategoryId = category.Id,
            ProductId = o,
        }).ToList();
        if (insertProductOnCategory.Count > 0) {
            addProductIds.AddRange(insertProductOnCategory.Select(o => o.ProductId).ToList());
            await db.ProductCategories.AddRangeAsync(insertProductOnCategory, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new ProdOnCategoryUAuditContext(request.MerchantId, request.UserId) {
            CategoryCode = category.Code,
            AddProductIds = addProductIds,
            DeleteProductIds = deleteProductIds,
        }, cancellationToken);
    }
}
