using Anch.Core;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Services;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

/// <summary>Handles the configurator request: run as.</summary>
public class RunAsHandler(IRunAsManager? runAsManager = null) : BaseWriteHandler, IRunAsHandler
{
    public async Task Execute(HttpContext context, CancellationToken ct) =>
        await runAsManager.FromMaybe(() => "RunAs not supported")
                          .StartRunAsUserAsync(await this.ParseRequestBodyAsync<string>(context), ct);
}
