using DR.Common.Exceptions;
using DR.Common.Hashers;
using DR.Resource;
using Microsoft.EntityFrameworkCore;

namespace DR.Handlers.UserHandlers {
    public class ResetPasswordUserReq : BaseReq {
        public string Password { get; set; } = null!;
    }

    public class ResetPasswordUserHandler : BaseHandler<ResetPasswordUserReq> {

        public ResetPasswordUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task Handle(ResetPasswordUserReq request, CancellationToken cancellationToken) {
            var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == request.UserId && o.MerchantId == request.MerchantId && !o.IsDelete && !o.IsSystem, cancellationToken);
            ManagedException.ThrowIfNull(user, Messages.User.ResetPassword.User_NotFound);

            user.Password = PasswordHasher.Hash(request.Password);

            await this.db.SaveChangesAsync(cancellationToken);
        }
    }
}
