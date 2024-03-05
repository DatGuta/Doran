using DR.Application.Handlers.RoleHandlers.Commands;
using DR.Application.Handlers.RoleHandlers.Queries;
using DR.WebManApi.Controllers.DrBase;

namespace DR.WebManApi.Controllers;

[ApiController, DrAuthorize(DrClaim.Web.Setting_Role, DrClaim.Web.Setting_User), Route("api/role")]
public class RoleController(IServiceProvider serviceProvider) : DrController(serviceProvider) {
    [HttpGet, DrAuthorize, Route("all")]
    public async Task<Result> All() {
        var data = await mediator.Send(new AllRoleQuery {
            UserId = userId,
        });
        return Result<AllRoleData>.Ok(data);
    }

    [HttpPost, DrAuthorize(DrClaim.Web.Setting_Role), Route("list")]
    public async Task<Result> List(ListRoleQuery req) {
        req.UserId = userId;
        var data = await mediator.Send(req);
        return Result<ListRoleData>.Ok(data);
    }

    [HttpPost, DrAuthorize(DrClaim.Web.Setting_Role), Route("get")]
    public async Task<Result> Get(GetRoleQuery req) {
        req.UserId = userId;
        var data = await mediator.Send(req);
        return Result<RoleDto?>.Ok(data);
    }

    [HttpGet, DrAuthorize(DrClaim.Web.Setting_Role), Route("permission")]
    public async Task<Result> Permission() {
        var data = await mediator.Send(new AllPermissionQuery());
        return Result<ListPermissionData>.Ok(data);
    }

    [HttpPost, DrAuthorize(DrClaim.Web.Setting_Role), Route("save")]
    public async Task<Result> Save([FromBody] RoleDto model) {
        var itemId = await mediator.Send(new SaveRoleCommand {
            UserId = userId,
            Model = model,
        });
        return Result<Guid>.Ok(itemId);
    }

    [HttpPost, DrAuthorize(DrClaim.Web.Setting_Role), Route("delete")]
    public async Task<Result> Delete(DeleteRoleCommand req) {
        req.UserId = userId;
        await mediator.Send(req);
        return Result.Ok();
    }
}
