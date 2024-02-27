using MediatR;
using Microsoft.AspNetCore.Mvc;
using DR.Common.Models;

namespace DR.ManageApi.Controllers {
    public abstract class BaseController : ControllerBase {
        protected readonly IMediator mediator;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly HttpContext? httpContext;

        protected BaseController(IServiceProvider serviceProvider) {
            this.mediator = serviceProvider.GetRequiredService<IMediator>();
            this.httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            this.httpContext = this.httpContextAccessor.HttpContext;
        }

        protected FileContentResult File(FileRes file) {
            return File(file.ByteArray, "application/octet-stream", file.FileName);
        }
    }
}
