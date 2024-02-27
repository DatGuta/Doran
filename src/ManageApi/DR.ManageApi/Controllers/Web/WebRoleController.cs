using DR.Attributes;
using DR.Common.Models;
using DR.Handlers.RoleHandlers;
using DR.ManageApi.Controllers;
using DR.Models;
using Microsoft.AspNetCore.Mvc;
using TuanVu.Handlers.RoleHandlers;

namespace DR.ManageApi.Controllers.Web {
    [ApiController, DRAuthorize(DrClaim.Web.Setting_Role, DrClaim.Web.Setting_User), Route("api/role")]
    public class WebRoleController : WebBaseController {

        public WebRoleController(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        [HttpGet, DRAuthorize, Route("all")]
        public async Task<BaseRes> All() {
            var data = await this.mediator.Send(new AllRoleReq {
                MerchantId = this.merchantId,
                UserId = this.userId,
            });
            return BaseRes<AllRoleData>.Ok(data);
        }

        [HttpPost, DRAuthorize(DrClaim.Web.Setting_Role), Route("list")]
        public async Task<BaseRes> List(ListRoleReq req) {
            req.MerchantId = this.merchantId;
            req.UserId = this.userId;
            var data = await this.mediator.Send(req);
            return BaseRes<ListRoleData>.Ok(data);
        }

        [HttpPost, DRAuthorize(DrClaim.Web.Setting_Role), Route("get")]
        public async Task<BaseRes> Get(GetRoleReq req) {
            req.MerchantId = this.merchantId;
            req.UserId = this.userId;
            var data = await this.mediator.Send(req);
            return BaseRes<RoleDto?>.Ok(data);
        }

        [HttpGet, DRAuthorize(DrClaim.Web.Setting_Role), Route("permission")]
        public async Task<BaseRes> Permission() {
            var data = await this.mediator.Send(new AllPermissionReq());
            return BaseRes<ListPermissionData>.Ok(data);
        }

        [HttpPost, DRAuthorize(DrClaim.Web.Setting_Role), Route("save")]
        public async Task<BaseRes> Save([FromBody] RoleDto model) {
            var itemId = await this.mediator.Send(new SaveRoleReq {
                MerchantId = this.merchantId,
                UserId = this.userId,
                Model = model,
            });
            return BaseRes<string>.Ok(itemId);
        }

        [HttpPost, DRAuthorize(DrClaim.Web.Setting_Role), Route("delete")]
        public async Task<BaseRes> Delete(DeleteRoleReq req) {
            req.MerchantId = this.merchantId;
            req.UserId = this.userId;
            await this.mediator.Send(req);
            return BaseRes.Ok();
        }
    }
}
