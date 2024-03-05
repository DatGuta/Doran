using System.Net;
using DR.Constant;
using DR.Helper;
using DR.Redis.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DR.Application;

public class SessionMiddleware(IServiceProvider serviceProvider) : IMiddleware {
    private readonly DrContext db = serviceProvider.GetRequiredService<DrContext>();
    private readonly IRedisService redisCacheService = serviceProvider.GetRequiredService<IRedisService>();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
        var userId = GetTokenGuid(context, Constants.TokenUserId);
        var source = GetTokenString(context, Constants.TokenSource);
        var session = GetTokenString(context, Constants.TokenSession);

        if (userId != Guid.Empty && !string.IsNullOrWhiteSpace(source) && long.TryParse(session, out long sessionValue)) {
            var lastSession = await GetLastSession(source, userId);
            if (sessionValue != lastSession) {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
            }
        }

        await next(context);
    }

    private async Task<long?> GetLastSession(string source, Guid userId) {
        string cacheKey = CacheKeyHelper.GetSessionKey(source, userId);
        if (!redisCacheService.TryGetValue(cacheKey, out long? sessionValue)) {
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(o => o.Id == userId);
            if (user != null) {
                sessionValue = user.LastSession;

                await redisCacheService.SetAsync(cacheKey, sessionValue);
            }
        }

        return sessionValue;
    }

    private static Guid GetTokenGuid(HttpContext context, string tokenKey) {
        return Guid.Parse(context.User.FindFirst(o => o.Type == tokenKey)?.Value ?? string.Empty);
    }

    private static string GetTokenString(HttpContext context, string tokenKey) {
        return context.User.FindFirst(o => o.Type == tokenKey)?.Value ?? string.Empty;
    }
}

