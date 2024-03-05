using DR.Application.Handlers.AuthHandlers;
using DR.Models.Dto;

namespace DR.Application.Handlers.RoleHandlers.Queries;

public class AllPermissionQuery : IRequest<ListPermissionData> { }

public class ListPermissionData : PaginatedList<PermissionDto> { }

public class ListPermissionHandler(IServiceProvider serviceProvider) :
    BaseHandler(serviceProvider), IRequestHandler<AllPermissionQuery, ListPermissionData> {
    public async Task<ListPermissionData> Handle(AllPermissionQuery request, CancellationToken cancellationToken) {
        var permissions = await this.db.GetPermissions(o => o.IsActive, cancellationToken);
        return new() { Items = PermissionDto.FromEntities(permissions)! };
    }
}
