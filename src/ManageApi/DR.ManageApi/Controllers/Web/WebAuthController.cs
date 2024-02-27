using DR.Common.Models;
using DR.Constant.Enums;
using DR.Handlers.AuthHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DR.ManageApi.Controllers.Web {
    [ApiController, AllowAnonymous, Route("api/auth")]
    public class WebAuthController : BaseController {

        public WebAuthController(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        [HttpPost, Route("login")]
        public async Task<BaseRes> Login(LoginReq req) {
            req.Permission = EPermission.Web;
            var res = await this.mediator.Send(req);
            return BaseRes<LoginRes>.Ok(res);
        }
    }
}
