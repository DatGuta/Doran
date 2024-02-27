using Microsoft.EntityFrameworkCore;
using DR.Common.Exceptions;
using DR.Common.Models;
using DR.Resource;
using DR.Common.Hashers;

namespace DR.Handlers.AuthHandlers {
    public class UnlockByPinCodeReq : BaseReq<bool> {
        public string PinCode { get; set; } = null!;
    }

    public class UnlockByPinCodeHandler : BaseHandler<UnlockByPinCodeReq, bool> {

        public UnlockByPinCodeHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task<bool> Handle(UnlockByPinCodeReq request, CancellationToken cancellationToken) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.UserId, cancellationToken);
            ManagedException.ThrowIfNull(user, Messages.Auth.Login.User_NotFound);
            ManagedException.ThrowIfFalse(PasswordHasher.Verify(request.PinCode, user.PinCode), Messages.Auth.Login.User_IncorrectPinCode);
            return true;
        }
    }
}
