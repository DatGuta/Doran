

using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Handlers;
using DR.Resource;

namespace TuanVu.Handlers.UserHandlers {
    public class ChangePinCodeUserReq : BaseReq {
        public string OldPinCode { get; set; } = null!;
        public string NewPinCode { get; set; } = null!;
    }

    public class ChangePinCodeUserHandler : BaseHandler<ChangePinCodeUserReq> {

        public ChangePinCodeUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task Handle(ChangePinCodeUserReq request, CancellationToken cancellationToken) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == request.UserId
                && o.MerchantId == request.MerchantId && !o.IsSystem, cancellationToken);
            ManagedException.ThrowIf(user == null, Messages.User.ChangePinCode.User_NotFound);
            ManagedException.ThrowIf(!PasswordHasher.Verify(request.OldPinCode, user.PinCode), Messages.User.ChangePinCode.User_IncorrentOldPinCode);

            user.PinCode = PasswordHasher.Hash(request.NewPinCode);

            await this.db.SaveChangesAsync(cancellationToken);
        }
    }
}
