using DR.Models;
using MediatR;

namespace DR.Handlers.RoleHandlers {
    public class AllPermissionReq : IRequest<ListPermissionData> { }

    public class ListPermissionData : BaseListData<PermissionDto> { }

    public class ListPermissionHandler : BaseHandler, IRequestHandler<AllPermissionReq, ListPermissionData> {

        public ListPermissionHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<ListPermissionData> Handle(AllPermissionReq request, CancellationToken cancellationToken) {
            var permissions = await this.db.GetPermissions(o => o.IsActive, cancellationToken);
            return new() { Items = PermissionDto.FromEntities(permissions)! };
        }
    }
}
