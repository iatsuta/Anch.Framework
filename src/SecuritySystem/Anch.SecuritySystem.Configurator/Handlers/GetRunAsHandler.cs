using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Services;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetRunAsHandler(IRunAsManager? runAsManager = null) : BaseReadHandler, IGetRunAsHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken) =>
        runAsManager?.RunAsUser?.Name ?? string.Empty;
}
