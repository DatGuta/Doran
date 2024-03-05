using DR.Application.Handlers.AuthHandlers;
using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Constant;

namespace DR.Application.Handlers.UserHandlers.Commands;

public class ChangePasswordUserCommand : Request {
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ChangePasswordUserHandler(IServiceProvider serviceProvider) : BaseHandler<ChangePasswordUserCommand>(serviceProvider) {
    public override async Task Handle(ChangePasswordUserCommand request, CancellationToken cancellationToken) {
        var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == request.UserId && !o.IsDelete && !o.IsSystem, cancellationToken);
        ManagedException.ThrowIf(user == null, Messages.User_NotFound);
        ManagedException.ThrowIf(!PasswordHasher.Verify(request.OldPassword, user.Password), Messages.User_IncorrectUserNameOrPassword);

        user.Password = PasswordHasher.Hash(request.NewPassword);
        await this.db.SaveChangesAsync(cancellationToken);
    }
}
