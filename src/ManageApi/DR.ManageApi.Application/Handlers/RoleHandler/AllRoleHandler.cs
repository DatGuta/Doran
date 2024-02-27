using DR.Models;
using Microsoft.EntityFrameworkCore;

namespace DR.Handlers.RoleHandlers {
    public class AllRoleReq : BaseReq<AllRoleData> {
    }

    public class AllRoleData : BaseListData<RoleDto> {
    }

    public class AllRoleHandler : BaseHandler<AllRoleReq, AllRoleData> {

        public AllRoleHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task<AllRoleData> Handle(AllRoleReq request, CancellationToken cancellationToken) {
            var items = await this.db.Roles.AsNoTracking().Where(o => o.MerchantId == request.MerchantId && !o.IsDelete)
                .OrderBy(o => o.Code).Select(o => RoleDto.FromEntity(o, null)!).ToListAsync(cancellationToken);
            return new() { Items = items, Count = items.Count };
        }
    }
}
