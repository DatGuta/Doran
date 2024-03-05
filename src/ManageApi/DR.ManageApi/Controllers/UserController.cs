using DR.Application.Handlers.UserHandlers.Commands;
using DR.Application.Handlers.UserHandlers.Queries;
using DR.WebManApi.Controllers.DrBase;

namespace DR.WebManApi.Controllers {
    [ApiController, DrAuthorize(DrClaim.Web.Setting_User), Route("api/user")]
    public class UserController(IServiceProvider serviceProvider) : DrController(serviceProvider) {
        [HttpGet, Route("all")]
        public async Task<Result> All() {
            var data = await mediator.Send(new AllUserQuery {
                UserId = userId,
            });
            return Result<ListUserData>.Ok(data);
        }

        [HttpPost, Route("list")]
        public async Task<Result> List(ListUserQuery req) {

            req.UserId = userId;
            var data = await mediator.Send(req);
            return Result<ListUserData>.Ok(data);
        }

        [HttpPost, Route("get")]
        public async Task<Result> Get(GetUserQuery req) {
            req.UserId = userId;
            var data = await mediator.Send(req);
            return Result<UserDto?>.Ok(data);
        }

        [HttpPost, Route("save")]
        public async Task<Result> Save(UserDto model) {
            await mediator.Send(new SaveUserCommand {
                UserId = userId,
                Model = model,
            });
            return Result.Ok();
        }

        [HttpPost, DrAuthorize(DrClaim.Web.Setting_User_Reset), Route("reset-password")]
        public async Task<Result> ResetPassword(ResetPasswordUserCommand req) {

            await mediator.Send(req);
            return Result.Ok();
        }



        [HttpPost, Route("delete")]
        public async Task<Result> Delete(DeleteUserCommand req) {

            req.UserId = userId;
            await mediator.Send(req);
            return Result.Ok();
        }
    }
}
