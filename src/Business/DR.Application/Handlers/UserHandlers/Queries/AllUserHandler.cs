using DR.Application.Handlers.AuthHandlers;
using DR.Models.Dto;

namespace DR.Application.Handlers.UserHandlers.Queries;

public class AllUserQuery : Request<ListUserData> { }

public class AllUserHandler(IServiceProvider serviceProvider) : BaseHandler<AllUserQuery, ListUserData>(serviceProvider) {
    public override async Task<ListUserData> Handle(AllUserQuery request, CancellationToken cancellationToken) {
        var items = await this.db.Users.AsNoTracking()
            .Where(o => !o.IsDelete && !o.IsSystem)
            .OrderBy(o => o.Username).Select(o => UserDto.FromEntity(o, null, null)).ToListAsync(cancellationToken);
        return new() { Items = items, Count = items.Count };
    }
}
