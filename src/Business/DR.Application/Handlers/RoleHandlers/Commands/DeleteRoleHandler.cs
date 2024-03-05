using DR.Application.Handlers.AuthHandlers;
using DR.Common.Exceptions;
using DR.Constant;

namespace DR.Application.Handlers.RoleHandlers.Commands;

public class DeleteRoleCommand : SingleRequest { }

public class DeleteRoleHandler(IServiceProvider serviceProvider) : BaseHandler<DeleteRoleCommand>(serviceProvider) {

    public override async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken) {
        var role = await this.db.Roles.Where(o => o.Id == request.Id && !o.IsDelete)
            .FirstOrDefaultAsync(cancellationToken);
        ManagedException.ThrowIfNull(role, Messages.Role_NotFound);

        var existedUser = await this.db.Users.AsNoTracking()
            .Where(o => o.RoleId == role.Id && !o.IsDelete)
            .AnyAsync(cancellationToken);
        ManagedException.ThrowIf(existedUser, Messages.Role_NotDelete);

        role.IsDelete = true;

        await this.db.SaveChangesAsync(cancellationToken);
    }
}
