using DR.Application.Handlers.AuthHandlers;
using DR.Common.Exceptions;
using DR.Common.Extensions;
using DR.Common.Hashers;
using DR.Constant;
using DR.Helper;
using DR.Models.Dto;

namespace DR.Application.Handlers.UserHandlers.Commands;

public class SaveUserCommand : ModelRequest<UserDto, Guid> {
    public bool IsSelfUpdate { get; set; }
}

public class SaveUserHandler(IServiceProvider serviceProvider) : BaseHandler<SaveUserCommand, Guid>(serviceProvider) {
    public override async Task<Guid> Handle(SaveUserCommand request, CancellationToken cancellationToken) {
        var userId = request.UserId;
        var model = request.Model;

        if (!request.IsSelfUpdate) {
            await this.Validate(model.Role?.Id);
        }

        if (model.Id.IfNullOrEmpty()) {
            model.Username = model.Username!.Trim().ToLower();
            return await this.Create(userId, model, cancellationToken);
        }
        return await this.Update(userId, model, request.IsSelfUpdate, cancellationToken);
    }

    private async Task<Guid> Create(Guid userId, UserDto model, CancellationToken cancellationToken) {
        var existed = await this.db.Users.AnyAsync(o => o.Username == model.Username && !o.IsDelete && !o.IsSystem, cancellationToken);
        ManagedException.ThrowIf(existed, Messages.User_Existed);
        ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User_NameIsRequire);

        var user = new Database.Models.User() {
            Id = Guid.NewGuid(),
            RoleId = model.Role!.Id,
            Username = model.Username!,
            Password = PasswordHasher.Hash(model.Password!),
            PinCode = PasswordHasher.Hash(model.PinCode!),
            Name = model.Name,
            SearchName = StringHelper.UnsignedUnicode(model.Name),
            Phone = model.Phone,
            Province = model.Province?.Code,
            District = model.District?.Code,
            Commune = model.Commune?.Code,
            Address = model.Address,
            IsSystem = false,
            IsAdmin = false,
        };
        await this.db.Users.AddAsync(user, cancellationToken);
        await this.db.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    private async Task<Guid> Update(Guid userId, UserDto model, bool isSelfUpdate, CancellationToken cancellationToken) {
        var existed = await this.db.Users.AnyAsync(o => o.Id != model.Id && o.Username == model.Username && !o.IsDelete && !o.IsSystem, cancellationToken);
        ManagedException.ThrowIf(existed, Messages.User_Existed);

        var user = await this.db.Users.FirstOrDefaultAsync(o => o.Id == model.Id && o.Username == model.Username && !o.IsDelete && !o.IsSystem, cancellationToken);
        ManagedException.ThrowIf(user == null, Messages.User_NotFound);
        ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.User_NameIsRequire);

        var originUser = user.Clone();

        user.Name = model.Name;
        user.SearchName = StringHelper.UnsignedUnicode(model.Name);
        user.Phone = model.Phone;
        user.Province = model.Province?.Code;
        user.District = model.District?.Code;
        user.Commune = model.Commune?.Code;
        user.Address = model.Address;
        if (!isSelfUpdate) {
            user.RoleId = model.Role!.Id;
        }

        await this.db.SaveChangesAsync(cancellationToken);
        return user.Id;
    }

    private async Task Validate(Guid? roleId) {
        ManagedException.ThrowIf(roleId.HasValue, Messages.Role_NotFound);
        var roleExisted = await this.db.Roles.AnyAsync(o => o.Id == roleId && !o.IsDelete);
        ManagedException.ThrowIf(!roleExisted, Messages.Role_NotFound);
    }
}
