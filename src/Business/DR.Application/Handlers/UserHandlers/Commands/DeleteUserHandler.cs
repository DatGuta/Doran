using DR.Application.Handlers.AuthHandlers;
using DR.Common.Exceptions;
using DR.Constant;

namespace DR.Application.Handlers.UserHandlers.Commands;

public class DeleteUserCommand : SingleRequest { }

public class DeleteUserHandler(IServiceProvider serviceProvider) : BaseHandler<DeleteUserCommand>(serviceProvider) {
    public override async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken) {
        var entity = await this.db.Users.Where(o => o.Id == request.Id && !o.IsDelete && !o.IsSystem).FirstOrDefaultAsync(cancellationToken);
        ManagedException.ThrowIfNull(entity, Messages.User_NotFound);

        entity.IsDelete = true;
        await this.db.SaveChangesAsync(cancellationToken);
    }
}
