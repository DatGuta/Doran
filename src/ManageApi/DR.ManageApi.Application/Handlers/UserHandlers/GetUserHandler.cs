
using DR.Database.Models;
using DR.Handlers;
using DR.Models;
using DR.Resource;

namespace TuanVu.Handlers.UserHandlers {
    public class GetUserReq : BaseSingleReq<UserDto?> { }

    public class GetUserHandler : BaseHandler<GetUserReq, UserDto?> {
        private readonly UnitRes unitRes;

        public GetUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.unitRes = serviceProvider.GetRequiredService<UnitRes>();
        }

        public override async Task<UserDto?> Handle(GetUserReq request, CancellationToken cancellationToken) {
            var user = await this.db.Users.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete && !o.IsSystem)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null) return null;

            Role? role = null;
            if (!string.IsNullOrEmpty(user.RoleId)) {
                role = await this.db.Roles.AsNoTracking()
                    .Where(o => o.MerchantId == request.MerchantId && o.Id == user.RoleId && !o.IsDelete)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            return UserDto.FromEntity(user, this.unitRes, role);
        }
    }
}
