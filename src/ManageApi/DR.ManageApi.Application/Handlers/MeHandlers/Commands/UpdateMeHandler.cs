using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Contexts.AuditContexts;
using DR.Helper;
using DR.Message;

namespace DR.ManageApi.Application.Handlers.MeHandlers.Commands;

public class UpdateMeCommand : ModelRequest<UserDto> { }

internal class UpdateMeHandler(IServiceProvider serviceProvider) : BaseHandler<UpdateMeCommand>(serviceProvider) {
    private readonly IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

    public override async Task Handle(UpdateMeCommand request, CancellationToken cancellationToken) {
        var model = request.Model;

        var user = await db.Users.FirstOrDefaultAsync(o => o.Id == request.UserId && !o.IsDelete && !o.IsSystem, cancellationToken);
        ManagedException.ThrowIf(user == null, Messages.User.CreateOrUpdate.User_NotFound);
        ManagedException.ThrowIf(user.IsAdmin && !model.IsActive, Messages.User.CreateOrUpdate.User_NotInactive);
        ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User.CreateOrUpdate.User_NameIsRequire);

        var originUser = user.Clone();

        user.Name = model.Name;
        user.SearchName = StringHelper.UnsignedUnicode(model.Name);
        user.Phone = model.Phone;
        user.Province = model.Province?.Code;
        user.District = model.District?.Code;
        user.Commune = model.Commune?.Code;
        user.Address = model.Address;

        await db.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new UserUAuditContext(request.MerchantId, request.UserId, originUser, user), cancellationToken);
    }
}
