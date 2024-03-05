using DR.Application.Handlers.AuthHandlers;
using DR.Database.Models;
using DR.Helper;
using DR.Models.Dto;
using DR.Resource;

namespace DR.Application.Handlers.UserHandlers.Queries;

public class GetUserQuery : SingleRequest<UserDto?> { }

public class GetUserHandler(IServiceProvider serviceProvider) : BaseHandler<GetUserQuery, UserDto?>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken) {
        var user = await this.db.Users.AsNoTracking()
            .Where(o => o.Id == request.Id && !o.IsDelete && !o.IsSystem)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null) return null;

        Role? role = null;
        if (!user.RoleId.IfNullOrEmpty()) {
            role = await this.db.Roles.AsNoTracking()
                .Where(o => o.Id == user.RoleId && !o.IsDelete)
                .FirstOrDefaultAsync(cancellationToken);
        }
        return UserDto.FromEntity(user, this.unitRes, role);
    }
}
