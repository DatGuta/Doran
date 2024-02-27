using DR.Models;
using DR.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DR.Handlers.UserHandlers {
    public class ListUserReq : BaseListReq<ListUserData> { }

    public class ListUserData : BaseListData<UserDto> { }

    public class ListUserHandler : BaseHandler<ListUserReq, ListUserData> {
        private readonly UnitRes unitRes;

        public ListUserHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.unitRes = serviceProvider.GetRequiredService<UnitRes>();
        }

        public override async Task<ListUserData> Handle(ListUserReq request, CancellationToken cancellationToken) {
            var query = this.db.Users.Include(o => o.Role).AsNoTracking()
                .Where(o => o.MerchantId == request.MerchantId && !o.IsDelete && !o.IsSystem)
                .WhereIf(!string.IsNullOrWhiteSpace(request.SearchText), o => o.Username.Contains(request.SearchText!.Trim().ToLower()));

            int count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken);

            UserDto? firstItem = null;
            if (string.IsNullOrWhiteSpace(request.SearchText) && !string.IsNullOrWhiteSpace(request.FirstItemId) && request.PageIndex == 0) {
                firstItem = await query.Where(o => o.Id == request.FirstItemId)
                    .Select(o => UserDto.FromEntity(o, this.unitRes, null))
                    .FirstOrDefaultAsync(cancellationToken);
                if (firstItem != null) {
                    request.PageSize--;
                    query = query.Where(o => o.Id != firstItem.Id);
                }
            }

            var items = await query.OrderByDescending(o => o.IsAdmin).ThenBy(o => o.Username)
            .Skip(request.PageIndex * request.PageSize).Take(request.PageSize)
                .Select(o => UserDto.FromEntity(o, unitRes, null)).ToListAsync(cancellationToken);

            if (firstItem != null) items.Insert(0, firstItem);

            return new() {
                Items = items!,
                Count = count,
            };
        }
    }
}
