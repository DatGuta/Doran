using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Lock;
using DR.Constant.Enums;
using DR.Contexts.AuditContexts;
using DR.Database.Models;
using DR.Helper;
using DR.ManageApi.Domain.Actions.AutoGenerate;
using DR.ManageApi.Domain.Services.Interfaces;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.CategoryHandlers.Commands;

public class SaveCategoryCommand : ModelRequest<CategoryDto, string> { }

internal class SaveCategoryHandler(IServiceProvider serviceProvider) : BaseHandler<SaveCategoryCommand, string>(serviceProvider) {
    private readonly IAutoGenerationService autoGenerationService = serviceProvider.GetRequiredService<IAutoGenerationService>();
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task<string> Handle(SaveCategoryCommand request, CancellationToken cancellationToken) {
        var merchantId = request.MerchantId;
        var userId = request.UserId;

        var model = request.Model;

        model.Code = model.Code?.Trim().ToUpper();
        model.Name = model.Name.Trim();

        return string.IsNullOrWhiteSpace(model.Id)
            ? await Locker.LockByKey($"Merchant:{request.MerchantId}:Category",
                () => Create(merchantId, userId, model, cancellationToken))
            : await Update(merchantId, userId, model, cancellationToken);
    }

    private async Task<string> Create(string merchantId, string userId, CategoryDto model, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(model.Code))
            model.Code = await autoGenerationService.GenerateCode(merchantId, ESetting.AutoGenerateCategoryCode, EAutoGenerator.CategoryCode, cancellationToken);
        else {
            var exited = await db.Categories.AnyAsync(o => o.MerchantId == merchantId && o.Code == model.Code, cancellationToken);
            ManagedException.ThrowIf(exited, Messages.Category.CreateOrUpdate.Category_Exited);
        }

        Category entity = new() {
            Id = NGuidHelper.New(),
            MerchantId = merchantId,
            Code = model.Code,
            Name = model.Name,
            SearchName = StringHelper.UnsignedUnicode(model.Name),
        };

        await db.Categories.AddAsync(entity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new CategoryCAuditContext(merchantId, userId, entity), cancellationToken);

        return entity.Id;
    }

    private async Task<string> Update(string merchantId, string userId, CategoryDto model, CancellationToken cancellationToken) {
        var exited = await db.Categories.AnyAsync(o => o.MerchantId == merchantId && o.Code == model.Code && o.Id != model.Id, cancellationToken);
        ManagedException.ThrowIf(exited, Messages.Category.CreateOrUpdate.Category_Exited);

        var entity = await db.Categories.FirstOrDefaultAsync(o => o.MerchantId == merchantId && !o.IsDelete && o.Id == model.Id, cancellationToken);
        ManagedException.ThrowIf(entity == null, Messages.Category.CreateOrUpdate.Category_NotFound);

        var originalEntity = entity.Clone();

        entity.Name = model.Name;
        entity.SearchName = StringHelper.UnsignedUnicode(model.Name);
        entity.ModifiedDate = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new CategoryUAuditContext(merchantId, userId, originalEntity, entity.Clone()), cancellationToken);

        return entity.Id;
    }
}
