using DR.Models;
using Microsoft.EntityFrameworkCore;

namespace DR.Handlers.RoleHandlers {
    public class GetRoleReq : BaseSingleReq<RoleDto?> {
    }

    public class GetRoleHandler : BaseHandler<GetRoleReq, RoleDto?> {

        public GetRoleHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public override async Task<RoleDto?> Handle(GetRoleReq request, CancellationToken cancellationToken) {
            var role = await this.db.Roles.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && o.Id == request.Id && !o.IsDelete)
                .FirstOrDefaultAsync(cancellationToken);
            if (role == null) return null;

            role.RolePermissions = await this.db.RolePermissions.AsNoTracking()
                .Where(o => o.RoleId == role.Id).ToListAsync(cancellationToken);

            var permissions = await this.db.GetPermissions(o => o.IsActive, cancellationToken);
            return RoleDto.FromEntity(role, permissions);
        }
    }
}
