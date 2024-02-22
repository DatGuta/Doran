using DR.Common.Exceptions;
using DR.Contexts.AuditContexts;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.CategoryHandlers.Commands;

public class DeleteCategoryCommand : SingleRequest { }

internal class DeleteCategoryHandler(IServiceProvider serviceProvider) : BaseHandler<DeleteCategoryCommand>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken) {
        var entity = await db.Categories.FirstOrDefaultAsync(
            o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete,
            cancellationToken
        );
        ManagedException.ThrowIf(entity == null, Messages.Category.Delete.Category_NotFound);

        entity.IsDelete = true;

        await db.SaveChangesAsync(cancellationToken);
        await mediator.Publish(new CategoryDAuditContext(request.MerchantId, request.UserId, entity), cancellationToken);
    }
}
