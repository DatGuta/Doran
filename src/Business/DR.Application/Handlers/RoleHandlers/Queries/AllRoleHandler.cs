using DR.Application.Handlers.AuthHandlers;
using DR.Models.Dto;

namespace DR.Application.Handlers.RoleHandlers.Queries;

public class AllRoleQuery : Request<AllRoleData> {
}

public class AllRoleData : PaginatedList<RoleDto> {
}

public class AllRoleHandler(IServiceProvider serviceProvider) : BaseHandler<AllRoleQuery, AllRoleData>(serviceProvider) {
    public override async Task<AllRoleData> Handle(AllRoleQuery request, CancellationToken cancellationToken) {
        var items = await this.db.Roles.AsNoTracking().Where(o => !o.IsDelete)
            .OrderBy(o => o.Code).Select(o => RoleDto.FromEntity(o, null)!).ToListAsync(cancellationToken);
        return new() { Items = items, Count = items.Count };
    }
}
