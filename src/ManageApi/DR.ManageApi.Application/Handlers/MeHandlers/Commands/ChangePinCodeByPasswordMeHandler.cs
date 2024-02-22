using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.MeHandlers.Commands;

public class ChangePinCodeByPasswordMeCommand : Request {
    public string Password { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
}

internal class ChangePinCodeByPasswordMeHandler(IServiceProvider serviceProvider) : BaseHandler<ChangePinCodeByPasswordMeCommand>(serviceProvider) {

    public override async Task Handle(ChangePinCodeByPasswordMeCommand request, CancellationToken cancellationToken) {
        var user = await db.Users.Where(o => o.Id == request.UserId && o.MerchantId == request.MerchantId && !o.IsDelete && !o.IsSystem)
            .FirstOrDefaultAsync(cancellationToken);
        ManagedException.ThrowIf(user == null, Messages.User.ChangePinCode.User_NotFound);
        ManagedException.ThrowIf(!PasswordHasher.Verify(request.Password, user.Password), Messages.User.ChangePinCode.User_IncorrentOldPassword);

        user.Password = PasswordHasher.Hash(request.PinCode);
        await db.SaveChangesAsync(cancellationToken);
    }
}
