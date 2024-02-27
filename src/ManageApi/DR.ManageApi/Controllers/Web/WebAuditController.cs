using Microsoft.AspNetCore.Mvc;
using DR.Attributes;
using DR.Common.Models;
using DR.Handlers.AuditHandlers;

namespace DR.ManageApi.Controllers.Web {
    [ApiController, DRAuthorize(DrClaim.Web.Audit), Route("api/audit")]
    public class WebAuditController : WebBaseController {

        public WebAuditController(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        [HttpPost, Route("list")]
        public async Task<BaseRes> List(ListAuditReq req) {
            req.MerchantId = this.merchantId;
            var data = await this.mediator.Send(req);
            return BaseRes<ListAuditData>.Ok(data);
        }
    }
}
