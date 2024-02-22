using DR.Constant.Enums;
using DR.ManageApi.Domain.Services.Interfaces;

namespace DR.ManageApi.Application.Handlers.BrandHandlers.Queries;

public class GetBrandQuery : SingleRequest<BrandDto?> { }

public class GetBrandHandler(IServiceProvider serviceProvider)
    : BaseHandler<GetBrandQuery, BrandDto?>(serviceProvider) {
    private readonly IImageService imageService = serviceProvider.GetRequiredService<IImageService>();

    public override async Task<BrandDto?> Handle(GetBrandQuery request, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(request.Id)) return null;

        var images = await this.imageService.List(request.MerchantId, EItemImage.Brand, request.Id);

        return await this.db.Brands.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete)
            .Select(o => BrandDto.FromEntity(o, images.FirstOrDefault(), this.url))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
