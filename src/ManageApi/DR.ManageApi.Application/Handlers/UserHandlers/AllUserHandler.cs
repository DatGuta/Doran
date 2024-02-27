using DR.Models;
using Microsoft.EntityFrameworkCore;
namespace DR.Handlers.UserHandlers {
    public class AllUserReq : BaseReq<ListUserData> { }

    public class AllUserHandler : BaseHandler<AllUserReq, ListUserData> {

        public AllUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task<ListUserData> Handle(AllUserReq request, CancellationToken cancellationToken) {
            var items = await this.db.Users.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete && !o.IsSystem)
                .OrderBy(o => o.Username).Select(o => UserDto.FromEntity(o, null, null)).ToListAsync(cancellationToken);
            return new() { Items = items, Count = items.Count };
        }
    }
}
