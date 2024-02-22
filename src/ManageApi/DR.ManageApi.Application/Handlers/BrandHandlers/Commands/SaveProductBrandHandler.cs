using DR.Contexts.AuditContexts;
using DR.Database.Models;
using DR.Helper;

namespace DR.ManageApi.Application.Handlers.BrandHandlers.Commands;

public class SaveProductBrandCommand : Request {
    public string BrandId { get; set; } = string.Empty;
    public List<string>? ProductIds { get; set; }
}

internal class SaveProductBrandHandler(IServiceProvider serviceProvider) : BaseHandler<SaveProductBrandCommand>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task Handle(SaveProductBrandCommand request, CancellationToken cancellationToken) {
        request.ProductIds ??= [];

        var brand = await db.Brands.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.BrandId, cancellationToken);
        if (brand == null) return;

        var items = await db.ProductBrands
            .Where(o => o.MerchantId == request.MerchantId && o.BrandId == request.BrandId)
            .ToListAsync(cancellationToken);

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
            }
        }

        var insertProductBrand = request.ProductIds.Where(o => !productIds.Contains(o)).Select(o => new ProductBrand() {
            Id = NGuidHelper.New(),
            MerchantId = request.MerchantId,
            BrandId = request.BrandId!,
            ProductId = o,
        }).ToList();

        if (insertProductBrand.Count > 0) {
            addProductIds.AddRange(insertProductBrand.Select(o => o.ProductId).ToList());
            await db.ProductBrands.AddRangeAsync(insertProductBrand, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new ProdBrandUAuditContext(request.MerchantId, request.UserId) {
            BrandCode = brand.Code,
            AddProductIds = addProductIds,
            DeleteProductIds = deleteProductIds,
        }, cancellationToken);
    }
}
