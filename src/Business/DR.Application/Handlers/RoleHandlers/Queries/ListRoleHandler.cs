using DR.Application.Handlers.AuthHandlers;
using DR.Database.Extensions;
using DR.Models.Dto;

namespace DR.Application.Handlers.RoleHandlers.Queries;

public class ListRoleQuery : PaginatedRequest<ListRoleData> { }

public class ListRoleData : PaginatedList<RoleDto> { }

public class ListRoleHandler(IServiceProvider serviceProvider) : BaseHandler<ListRoleQuery, ListRoleData>(serviceProvider) {
    public override async Task<ListRoleData> Handle(ListRoleQuery request, CancellationToken cancellationToken) {
        var query = this.db.Roles.AsNoTracking().Where(o => !o.IsDelete)
            .WhereFunc(!string.IsNullOrWhiteSpace(request.SearchText), q => {
                var searchText = request.SearchText!.ToLowerInvariant().Trim().ToLower();
                return q.Where(o => o.Code.Contains(searchText.ToUpper()) || o.SearchName.Contains(searchText));
            });

        int count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);
        var items = await query.OrderBy(o => o.Code)
            .Skip(request.PageIndex * request.PageSize).Take(request.PageSize)
            .Select(o => RoleDto.FromEntity(o, null)!).ToListAsync(cancellationToken);


        return new() {
            Items = items,
            Count = count,
        };
    }
}
