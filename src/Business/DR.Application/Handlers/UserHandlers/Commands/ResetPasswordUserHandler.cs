using DR.Application.Handlers.AuthHandlers;
using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Constant;

namespace DR.Application.Handlers.UserHandlers.Commands;

public class ResetPasswordUserCommand : Request {
    public string Password { get; set; } = null!;
}

public class ResetPasswordUserHandler(IServiceProvider serviceProvider) : BaseHandler<ResetPasswordUserCommand>(serviceProvider) {
    public override async Task Handle(ResetPasswordUserCommand request, CancellationToken cancellationToken) {
        var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == request.UserId && !o.IsDelete && !o.IsSystem, cancellationToken);
        ManagedException.ThrowIfNull(user, Messages.User_NotFound);

        user.Password = PasswordHasher.Hash(request.Password);
        await this.db.SaveChangesAsync(cancellationToken);
    }
}
