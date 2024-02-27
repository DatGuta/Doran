using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Resource;
using Microsoft.EntityFrameworkCore;

namespace DR.Handlers.UserHandlers {
    public class ChangePasswordUserReq : BaseReq {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class ChangePasswordUserHandler : BaseHandler<ChangePasswordUserReq> {

        public ChangePasswordUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task Handle(ChangePasswordUserReq request, CancellationToken cancellationToken) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == request.UserId
                && o.MerchantId == request.MerchantId && !o.IsDelete && !o.IsSystem, cancellationToken);
            ManagedException.ThrowIf(user == null, Messages.User.ChangePassword.User_NotFound);
            ManagedException.ThrowIf(!PasswordHasher.Verify(request.OldPassword, user.Password), Messages.User.ChangePassword.User_IncorrentOldPassword);

            user.Password = PasswordHasher.Hash(request.NewPassword);

            await this.db.SaveChangesAsync(cancellationToken);
        }
    }
}
