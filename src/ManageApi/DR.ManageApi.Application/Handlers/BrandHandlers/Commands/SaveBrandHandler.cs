using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Lock;
using DR.Constant.Enums;
using DR.Contexts.AuditContexts;
using DR.ManageApi.Domain.Actions.AutoGenerate;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;

namespace FMS.ManageApi.Application.Handlers.BrandHandlers.Commands;

public class SaveBrandCommand : ModelRequest<BrandDto, string> { }

internal class SaveBrandHandler(IServiceProvider serviceProvider)
    : BaseHandler<SaveBrandCommand, string>(serviceProvider) {
    private readonly IImageService imageService = serviceProvider.GetRequiredService<IImageService>();
    private readonly IAutoGenerationService autoGenerationService = serviceProvider.GetRequiredService<IAutoGenerationService>();
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task<string> Handle(SaveBrandCommand request, CancellationToken cancellationToken) {
        var merchantId = request.MerchantId;
        var userId = request.UserId;
        var model = request.Model;
        model.Code = model.Code?.Trim().ToUpper();
        model.Name = model.Name.Trim();
        model.Phone = model.Phone?.Trim();
        model.Email = model.Email?.Trim();

        return string.IsNullOrWhiteSpace(model.Id)
            ? await Locker.LockByKey($"Merchant:{merchantId}:Brand",
                () => this.Create(merchantId, userId, model, cancellationToken))
            : await this.Update(merchantId, userId, model, cancellationToken);
    }

    private async Task<string> Create(string merchantId, string userId, BrandDto model, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(model.Code)) {
            model.Code = await this.autoGenerationService.GenerateCode(merchantId, ESetting.AutoGenerateBrandCode, EAutoGenerator.BrandCode, cancellationToken);
        } else {
            var existed = await this.db.Brands.AnyAsync(o => o.MerchantId == merchantId && o.Code == model.Code, cancellationToken);
            ManagedException.ThrowIf(existed, Messages.Brand.CreateOrUpdate.Brand_Existed);
        }

        var entity = model.ToEntity(merchantId);

        if (model.Image != null) {
            await this.imageService.Save(merchantId, EItemImage.Brand, entity.Id, model.Image);
        }

        await this.db.Brands.AddAsync(entity, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);

        await this.mediator.Publish(new BrandCAuditContext(merchantId, userId, entity), cancellationToken);

        return entity.Id;
    }

    private async Task<string> Update(string merchantId, string userId, BrandDto model, CancellationToken cancellationToken) {
        var existed = await this.db.Brands.AnyAsync(o => o.MerchantId == merchantId && o.Id != model.Id && o.Code == model.Code, cancellationToken);
        ManagedException.ThrowIf(existed, Messages.Brand.CreateOrUpdate.Brand_Existed);

        var entity = await this.db.Brands.FirstOrDefaultAsync(o => o.MerchantId == merchantId && o.Id == model.Id && !o.IsDelete, cancellationToken);
        ManagedException.ThrowIfNull(entity, Messages.Brand.CreateOrUpdate.Brand_NotFound);

        var originalEntity = entity.Clone();

        entity.UpdateFrom(model.ToEntity(merchantId));

        if (model.Image != null) {
            var itemImages = await this.imageService.List(merchantId, EItemImage.Brand, entity.Id, false);
            var itemImage = itemImages.Find(o => o.Id == model.Image.Id);
            await this.imageService.Save(merchantId, EItemImage.Brand, entity.Id, model.Image, entity: itemImage);
        }

        await this.db.SaveChangesAsync(cancellationToken);

        await this.mediator.Publish(new BrandUAuditContext(merchantId, userId, originalEntity, entity), cancellationToken);

        return entity.Id;
    }
}
