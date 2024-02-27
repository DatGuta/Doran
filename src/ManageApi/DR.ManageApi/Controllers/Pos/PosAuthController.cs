using DR.Attributes;
using DR.Common.Models;
using DR.Constant.Enums;
using DR.Handlers.AuthHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DR.ManageApi.Controllers.Pos {
    [ApiController, AllowAnonymous, Route("api/pos/auth")]
    public class PosAuthController : PosBaseController {

        public PosAuthController(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        [HttpPost, Route("login")]
        public async Task<BaseRes> Login(LoginReq req) {
            req.Permission = EPermission.POS;
            var data = await this.mediator.Send(req);
            return BaseRes<LoginRes>.Ok(data);
        }

        //[HttpPost, DRAuthorize, Route("check")]
        //public async Task<BaseRes> Check() {
        //    var valid = await this.mediator.Send(new PosCheckReq() {
        //        MerchantId = this.merchantId,
        //        StoreId = this.storeId,
        //        WarehouseId = this.warehouseId
        //    });
        //    return valid ? BaseRes.Ok() : BaseRes.Fail();
        //}

        [HttpPost, DRAuthorize, Route("unlock-by-pin")]
        public async Task<BaseRes> UnlockByPinCode(UnlockByPinCodeReq req) {
            req.MerchantId = this.merchantId;
            req.UserId = this.userId;
            var valid = await this.mediator.Send(req);
            return valid ? BaseRes.Ok() : BaseRes.Fail();
        }
    }
}
