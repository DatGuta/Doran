using DR.Common.Exceptions;
using DR.Contexts.AuditContexts;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.BrandHandlers.Commands;

public class DeleteBrandCommand : SingleRequest { }

internal class DeleteBrandHandler(IServiceProvider serviceProvider)
    : BaseHandler<DeleteBrandCommand>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken) {
        var entity = await this.db.Brands.FirstOrDefaultAsync(o => o.Id == request.Id && o.MerchantId == request.MerchantId && !o.IsDelete, cancellationToken);
        ManagedException.ThrowIfNull(entity, Messages.Brand.Delete.Brand_NotFound);

        var productExisted = await this.db.ProductBrands.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete && o.BrandId == entity.Id)
            .Select(o => o.Id).AnyAsync(cancellationToken);
        ManagedException.ThrowIf(productExisted, Messages.Brand.Delete.Brand_OnUsing);

        entity.IsDelete = true;

        await this.db.SaveChangesAsync(cancellationToken);

        await this.mediator.Publish(new BrandDAuditContext(request.MerchantId, request.UserId, entity), cancellationToken);
    }
}
