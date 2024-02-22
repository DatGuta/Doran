using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.MeHandlers.Commands;
public class ChangePinCodeMeCommand : Request {
    public string OldPinCode { get; set; } = string.Empty;
    public string NewPinCode { get; set; } = string.Empty;
}

internal class ChangePinCodeMeHandler(IServiceProvider serviceProvider) : BaseHandler<ChangePinCodeMeCommand>(serviceProvider) {

    public override async Task Handle(ChangePinCodeMeCommand request, CancellationToken cancellationToken) {
        var user = await db.Users.Where(o => o.Id == request.UserId && o.MerchantId == request.MerchantId && !o.IsSystem)
            .FirstOrDefaultAsync(cancellationToken);
        ManagedException.ThrowIf(user == null, Messages.User.ChangePinCode.User_NotFound);
        ManagedException.ThrowIf(!PasswordHasher.Verify(request.OldPinCode, user.PinCode), Messages.User.ChangePinCode.User_IncorrentOldPinCode);

        user.PinCode = PasswordHasher.Hash(request.NewPinCode);
        await db.SaveChangesAsync(cancellationToken);
    }
}
