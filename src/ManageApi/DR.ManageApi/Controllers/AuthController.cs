using DR.Application.Handlers.AuthHandlers.Commands;
using DR.Constant.Enums;
using DR.WebManApi.Controllers.DrBase;
using Microsoft.AspNetCore.Authorization;

namespace DR.WebManApi.Controllers {

    [ApiController, AllowAnonymous, Route("api/auth")]
    public class AuthController(IServiceProvider serviceProvider) : DrController(serviceProvider) {
        [HttpPost, Route("login")]
        public async Task<Result> Login(LoginCommand req) {
            req.Permission = EPermission.Web;
            var res = await mediator.Send(req);
            return Result<LoginResult>.Ok(res);
        }
    }
}
