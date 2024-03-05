using DR.Application.Handlers.AuthHandlers;
using DR.Database.Extensions;
using DR.Models.Dto;
using DR.Resource;

namespace DR.Application.Handlers.UserHandlers.Queries;

public class ListUserQuery : PaginatedRequest<ListUserData> { }

public class ListUserData : PaginatedList<UserDto> { }

public class ListUserHandler(IServiceProvider serviceProvider) : BaseHandler<ListUserQuery, ListUserData>(serviceProvider) {
    private readonly UnitResource unitRes = serviceProvider.GetRequiredService<UnitResource>();

    public override async Task<ListUserData> Handle(ListUserQuery request, CancellationToken cancellationToken) {
        var query = this.db.Users.Include(o => o.Role).AsNoTracking()
            .Where(o => !o.IsDelete && !o.IsSystem)
            .WhereIf(!string.IsNullOrWhiteSpace(request.SearchText), o => o.Username.Contains(request.SearchText!.Trim().ToLower()));

        int count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);
        var items = await query.OrderByDescending(o => o.IsAdmin).ThenBy(o => o.Username)
            .Skip(request.PageIndex * request.PageSize).Take(request.PageSize)
            .Select(o => UserDto.FromEntity(o, unitRes, null)).ToListAsync(cancellationToken);


        return new() {
            Items = items!,
            Count = count,
        };
    }
}
