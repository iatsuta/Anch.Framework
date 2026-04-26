using Anch.Core;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Services;
using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class RunAsHandler(IRunAsManager? runAsManager = null) : BaseWriteHandler, IRunAsHandler
{
    public async Task Execute(HttpContext context, CancellationToken cancellationToken) =>
        await runAsManager.FromMaybe(() => "RunAs not supported")
                          .StartRunAsUserAsync(await this.ParseRequestBodyAsync<string>(context), cancellationToken);
}
