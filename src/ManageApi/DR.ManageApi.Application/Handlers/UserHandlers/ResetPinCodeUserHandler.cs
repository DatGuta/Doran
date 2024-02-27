using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Handlers;
using DR.Resource;
using Microsoft.EntityFrameworkCore;

namespace TuanVu.Handlers.UserHandlers {
    public class ResetPinCodeUserReq : BaseReq {
        public string PinCode { get; set; } = null!;
    }

    public class ResetPinCodeUserHandler : BaseHandler<ResetPinCodeUserReq> {

        public ResetPinCodeUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task Handle(ResetPinCodeUserReq request, CancellationToken cancellationToken) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.MerchantId == request.MerchantId && o.Id == request.UserId && !o.IsDelete && !o.IsSystem, cancellationToken);
            ManagedException.ThrowIfNull(user, Messages.User.ResetPinCode.User_NotFound);

            user.PinCode = PasswordHasher.Hash(request.PinCode);

            await this.db.SaveChangesAsync(cancellationToken);
        }
    }
}
