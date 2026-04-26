using Anch.Core.Auth;
using Microsoft.AspNetCore.Http;

namespace ExampleApp.Infrastructure.Services;

public class ExampleDefaultCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Name => httpContextAccessor.HttpContext?.User.Identity?.Name ?? "system";
}