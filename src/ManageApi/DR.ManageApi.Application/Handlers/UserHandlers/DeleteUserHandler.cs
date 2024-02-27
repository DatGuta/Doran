using DR.Common.Exceptions;
using DR.Contexts.AuditContexts;
using DR.Handlers;
using DR.Resource;
using Microsoft.EntityFrameworkCore;

namespace TuanVu.Handlers.UserHandlers {

    public class DeleteUserReq : BaseSingleReq { }

    public class DeleteUserHandler : BaseHandler<DeleteUserReq> {

        public DeleteUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task Handle(DeleteUserReq request, CancellationToken cancellationToken) {
            var entity = await this.db.Users.Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete && !o.IsSystem).FirstOrDefaultAsync(cancellationToken);
            ManagedException.ThrowIfNull(entity, Messages.User.Delete.User_NotFound);
            ManagedException.ThrowIf(entity.IsActive, Messages.User.Delete.User_NotDelete);

            entity.IsDelete = true; 

            await this.db.SaveChangesAsync(cancellationToken);
            await this.mediator.Publish(new UserDAuditContext(request.MerchantId, request.UserId, entity), cancellationToken);
        }
    }
}
