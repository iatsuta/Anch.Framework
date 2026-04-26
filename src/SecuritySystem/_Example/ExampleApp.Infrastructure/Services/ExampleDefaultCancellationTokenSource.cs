using Anch.Core;

using Microsoft.AspNetCore.Http;

namespace ExampleApp.Infrastructure.Services;

public class ExampleDefaultCancellationTokenSource(IHttpContextAccessor httpContextAccessor) : IDefaultCancellationTokenSource
{
    public CancellationToken CancellationToken => httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
}