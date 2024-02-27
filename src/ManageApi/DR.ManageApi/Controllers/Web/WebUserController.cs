using DR.Attributes;
using DR.Common.Models;
using DR.Handlers.UserHandlers;
using DR.Models;
using Microsoft.AspNetCore.Mvc;
using TuanVu.Handlers.UserHandlers;

namespace DR.ManageApi.Controllers.Web {
    [ApiController, DRAuthorize(DrClaim.Web.Setting_User), Route("api/user")]
    public class WebUserController : WebBaseController {

        public WebUserController(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        [HttpGet, Route("all")]
        public async Task<BaseRes> All() {
            var data = await this.mediator.Send(new AllUserReq {
                MerchantId = this.merchantId,
                UserId = this.userId,
            });
            return BaseRes<ListUserData>.Ok(data);
        }

        [HttpPost, Route("list")]
        public async Task<BaseRes> List(ListUserReq req) {
            req.MerchantId = this.merchantId;
            req.UserId = this.userId;
            var data = await this.mediator.Send(req);
            return BaseRes<ListUserData>.Ok(data);
        }

        [HttpPost, Route("get")]
        public async Task<BaseRes> Get(GetUserReq req) {
            req.MerchantId = this.merchantId;
            req.UserId = this.userId;
            var data = await this.mediator.Send(req);
            return BaseRes<UserDto?>.Ok(data);
        }

        [HttpPost, Route("save")]
        public async Task<BaseRes> Save(UserDto model) {
            await this.mediator.Send(new SaveUserReq {
                MerchantId = this.merchantId,
                UserId = this.userId,
                Model = model,
            });
            return BaseRes.Ok();
        }

        [HttpPost, DRAuthorize(DrClaim.Web.Setting_User_Reset), Route("reset-password")]
        public async Task<BaseRes> ResetPassword(ResetPasswordUserReq req) {
            req.MerchantId = this.merchantId;
            await this.mediator.Send(req);
            return BaseRes.Ok();
        }

        [HttpPost, DRAuthorize(DrClaim.Web.Setting_User_Reset), Route("reset-pincode")]
        public async Task<BaseRes> ResetPinCode(ResetPinCodeUserReq req) {
            req.MerchantId = this.merchantId;
            await this.mediator.Send(req);
            return BaseRes.Ok();
        }

        [HttpPost, Route("delete")]
        public async Task<BaseRes> Delete(DeleteUserReq req) {
            req.MerchantId = this.merchantId;
            req.UserId = this.userId;
            await this.mediator.Send(req);
            return BaseRes.Ok();
        }
    }
}
