using DR.Resource;

namespace DR.ManageApi.Application.Handlers.MeHandlers.Queries;

public class GetMeQuery : ModelRequest<UserDto, UserDto?> { }

internal class GetMeHandler(IServiceProvider serviceProvider) : BaseHandler<GetMeQuery, UserDto?>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<UserDto?> Handle(GetMeQuery request, CancellationToken cancellationToken) {
        var user = await db.Users.AsNoTracking()
            .Where(o => o.MerchantId == request.MerchantId && o.Id == request.UserId && !o.IsDelete && !o.IsSystem)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null) return null;

        Database.Models.Role? role = null;
        if (!string.IsNullOrEmpty(user.RoleId)) {
            role = await db.Roles.AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && o.Id == user.RoleId && !o.IsDelete)
                .FirstOrDefaultAsync(cancellationToken);
        }
        return UserDto.FromEntity(user, unitRes, role);
    }
}
