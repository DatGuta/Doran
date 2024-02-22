using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.MeHandlers.Commands;
public class ChangePasswordMeCommand : Request {
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

internal class ChangePasswordMeHandler(IServiceProvider serviceProvider) : BaseHandler<ChangePasswordMeCommand>(serviceProvider) {

    public override async Task Handle(ChangePasswordMeCommand request, CancellationToken cancellationToken) {
        var user = await db.Users.Where(o => o.Id == request.UserId && o.MerchantId == request.MerchantId && !o.IsDelete && !o.IsSystem)
            .FirstOrDefaultAsync(cancellationToken);
        ManagedException.ThrowIf(user == null, Messages.User.ChangePassword.User_NotFound);
        ManagedException.ThrowIf(!PasswordHasher.Verify(request.OldPassword, user.Password), Messages.User.ChangePassword.User_IncorrentOldPassword);

        user.Password = PasswordHasher.Hash(request.NewPassword);
        await db.SaveChangesAsync(cancellationToken);
    }
}
