using DR.Constant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DR.WebManApi.Controllers.DrBase {
    public abstract class DrController : ControllerBase {
        protected readonly IMediator mediator;
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected readonly HttpContext? httpContext;

        protected readonly Guid userId;

        protected DrController(IServiceProvider serviceProvider) {
            mediator = serviceProvider.GetRequiredService<IMediator>();
            httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            httpContext = httpContextAccessor.HttpContext;
            userId = GetToken(Constants.TokenUserId);
        }

        protected FileContentResult File(Common.Models.FileResult file) {
            return File(file.ByteArray, "application/octet-stream", file.FileName);
        }

        protected Guid GetToken(string key) {
            return Guid.Parse(httpContext?.User?.FindFirst(o => o.Type == key)?.Value ?? string.Empty);
        }
    }
}
