using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Resource;
using Microsoft.EntityFrameworkCore;
namespace DR.Handlers.UserHandlers {
    public class ChangePinCodeByPasswordUserReq : BaseReq {
        public string Password { get; set; } = null!;
        public string PinCode { get; set; } = null!;
    }

    public class ChangePinCodeByPasswordUserHandler : BaseHandler<ChangePinCodeByPasswordUserReq> {

        public ChangePinCodeByPasswordUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task Handle(ChangePinCodeByPasswordUserReq request, CancellationToken cancellationToken) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == request.UserId && o.MerchantId == request.MerchantId
                && !o.IsDelete && !o.IsSystem, cancellationToken);
            ManagedException.ThrowIf(user == null, Messages.User.ChangePinCode.User_NotFound);
            ManagedException.ThrowIf(!PasswordHasher.Verify(request.Password, user.Password), Messages.User.ChangePinCode.User_IncorrentOldPassword);

            user.Password = PasswordHasher.Hash(request.PinCode);

            await this.db.SaveChangesAsync(cancellationToken);
        }
    }
}
