using Anch.Core;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Services;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

/// <summary>Handles the configurator request: stop run as.</summary>
public class StopRunAsHandler(IRunAsManager? runAsManager = null) : BaseWriteHandler, IStopRunAsHandler
{
    public async Task Execute(HttpContext context, CancellationToken ct) =>
        await runAsManager.FromMaybe(() => "RunAs not supported").FinishRunAsUserAsync(ct);
}
