using DR.Constant.Enums;
using DR.Helper;
using DR.ManageApi.Domain.Services.Interfaces;

namespace FMS.ManageApi.Application.Handlers.BrandHandlers.Queries;

public class ListBrandQuery : PaginatedRequest<ListBrandData> {
    public bool IsAll { get; set; }
}

public class ListBrandData : PaginatedList<BrandDto> { }

public class ListBrandHandler(IServiceProvider serviceProvider)
    : BaseHandler<ListBrandQuery, ListBrandData>(serviceProvider) {
    private readonly IImageService imageService = serviceProvider.GetRequiredService<IImageService>();

    public override async Task<ListBrandData> Handle(ListBrandQuery request, CancellationToken cancellationToken) {
        var query = this.db.Brands.AsNoTracking().Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
                .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
                    var searchText = StringHelper.UnsignedUnicode(request.SearchText!);
                    return q.Where(o => o.Code.Contains(searchText.ToUpper())
                        || o.SearchName.Contains(searchText)
                        || (!string.IsNullOrEmpty(o.Phone) && o.Phone.ToLower().Contains(searchText))
                        || (!string.IsNullOrEmpty(o.Email) && o.Email.ToLower().Contains(searchText)));
                });

        int count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);

        BrandDto? firstItem = null;
        if (string.IsNullOrWhiteSpace(request.SearchText) && !string.IsNullOrWhiteSpace(request.FirstItemId) && request.PageIndex == 0) {
            firstItem = await query.Where(o => o.Id == request.FirstItemId)
                .Select(o => BrandDto.FromEntity(o, null, this.url))
                .FirstOrDefaultAsync(cancellationToken);

            if (firstItem != null) {
                request.PageSize--;
                query = query.Where(o => o.Id != firstItem.Id);
            }
        }

        query = query.OrderBy(o => o.Code);
        if (!request.IsAll) {
            query = query.Skip(request.Skip).Take(request.Take);
        }

        var items = await query.Select(o => BrandDto.FromEntity(o, null, this.url)).ToListAsync(cancellationToken);
        if (firstItem != null) items.Insert(0, firstItem);

        if (items.Count > 0) {
            var itemIds = items.Select(o => o.Id!).ToList();
            var images = await this.imageService.List(request.MerchantId, EItemImage.Brand, itemIds);
            if (images.Count > 0) {
                var productImages = images.GroupBy(o => o.ItemId)
                    .ToDictionary(k => k.Key, v => v.Select(o => ImageDto.FromEntity(o, this.url)).ToList());
                foreach (var item in items) {
                    item.Image = productImages.TryGetValue(item.Id!, out var itemImages) ? itemImages.FirstOrDefault() : default;
                }
            }
        }

        return new() {
            Items = items,
            Count = count,
        };
    }
}
