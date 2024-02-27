using DR.Common.Exceptions;
using DR.Contexts;
using DR.Handlers;
using DR.Resource;
using Microsoft.EntityFrameworkCore;

namespace TuanVu.Handlers.RoleHandlers {

    public class DeleteRoleReq : BaseSingleReq { }

    public class DeleteRoleHandler : BaseHandler<DeleteRoleReq> {

        public DeleteRoleHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task Handle(DeleteRoleReq request, CancellationToken cancellationToken) {
            var role = await this.db.Roles.Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete)
                .FirstOrDefaultAsync(cancellationToken);
            ManagedException.ThrowIfNull(role, Messages.Role.Delete.Role_NotFound);

            var existedUser = await this.db.Users.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && o.RoleId == role.Id && !o.IsDelete)
                .AnyAsync(cancellationToken);
            ManagedException.ThrowIf(existedUser, Messages.Role.Delete.Role_NotDelete);

            role.IsDelete = true;

            await this.db.SaveChangesAsync(cancellationToken);
            await this.mediator.Publish(new RoleDAuditContext(request.MerchantId, request.UserId, role), cancellationToken);
        }
    }
}
