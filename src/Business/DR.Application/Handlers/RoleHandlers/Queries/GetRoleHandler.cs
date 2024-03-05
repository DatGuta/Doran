using DR.Application.Handlers.AuthHandlers;
using DR.Models.Dto;

namespace DR.Application.Handlers.RoleHandlers.Queries;

public class GetRoleQuery : SingleRequest<RoleDto?> { }

public class GetRoleHandler : BaseHandler<GetRoleQuery, RoleDto?> {

    public GetRoleHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
    }

    public override async Task<RoleDto?> Handle(GetRoleQuery request, CancellationToken cancellationToken) {
        var role = await this.db.Roles.AsNoTracking()
            .Where(o => o.Id == request.Id && !o.IsDelete)
            .FirstOrDefaultAsync(cancellationToken);
        if (role == null) return null;

        role.RolePermissions = await this.db.RolePermissions.AsNoTracking()
            .Where(o => o.RoleId == role.Id).ToListAsync(cancellationToken);

        var permissions = await this.db.GetPermissions(o => o.IsActive, cancellationToken);
        return RoleDto.FromEntity(role, permissions);
    }
}
