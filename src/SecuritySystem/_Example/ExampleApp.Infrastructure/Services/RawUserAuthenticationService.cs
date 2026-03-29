using CommonFramework.Auth;

using Microsoft.AspNetCore.Http;

namespace ExampleApp.Infrastructure.Services;

public class ExampleRawCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Name => httpContextAccessor.HttpContext?.User.Identity?.Name ?? "system";
}