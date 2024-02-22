
using DR.Constant.Enums;

namespace DR.ManageApi.Application.Handlers.AuditHandlers.Queries;

public class ListAuditQuery : PaginatedRequest<ListAuditData> {
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public List<string> UserIds { get; set; } = [];
    public List<EAuditAction> Actions { get; set; } = [];
    public List<EAuditDocType> DocTypes { get; set; } = [];
}

public class ListAuditData : PaginatedList<AuditDto> { }

public class ListAuditHandler(IServiceProvider serviceProvider)
    : BaseHandler<ListAuditQuery, ListAuditData>(serviceProvider) {

    public override async Task<ListAuditData> Handle(ListAuditQuery request, CancellationToken cancellationToken) {
        request.UserIds ??= [];
        request.Actions ??= [];
        request.DocTypes ??= [];

        var query = this.db.UserAudits.AsNoTracking().Where(o => o.MerchantId == request.MerchantId)
            .WhereIf(request.From.HasValue, o => request.From!.Value <= o.Time)
            .WhereIf(request.To.HasValue, o => o.Time <= request.To!.Value)
            .WhereIf(request.UserIds.Count > 0, o => request.UserIds.Contains(o.UserId))
            .WhereIf(request.Actions.Count > 0, o => request.Actions.Contains(o.Action))
            .WhereIf(request.DocTypes.Count > 0, o => request.DocTypes.Contains(o.DocType))
            .WhereIf(!string.IsNullOrWhiteSpace(request.SearchText),
                o => !string.IsNullOrWhiteSpace(o.DocCode)
                    && o.DocCode.Contains(request.SearchText!.Trim(), StringComparison.CurrentCultureIgnoreCase));

        var audits = await query.OrderByDescending(o => o.Time)
            .Skip(request.PageIndex * request.PageSize).Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userIds = audits.Select(o => o.UserId).Distinct().ToList();
        var users = await this.db.Users.AsNoTracking().Where(o => userIds.Contains(o.Id)).ToListAsync(cancellationToken);
        var userMap = users.ToDictionary(k => k.Id, v => UserDto.FromEntity(v, null));

        var items = audits.Select(o => {
            var item = AuditDto.FromEntity(o);
            item.User = userMap.GetValueOrDefault(o.UserId);
            return item;
        }).ToList();

        return new() {
            Items = items!,
            Count = await query.CountIf(request.IsCount, o => o.Id, cancellationToken)
        };
    }
}
