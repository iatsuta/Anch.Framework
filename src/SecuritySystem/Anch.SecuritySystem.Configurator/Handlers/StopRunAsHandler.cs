using Anch.Core;
using Anch.SecuritySystem.Configurator.Interfaces;
using Anch.SecuritySystem.Services;
using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class StopRunAsHandler(IRunAsManager? runAsManager = null) : BaseWriteHandler, IStopRunAsHandler
{
    public async Task Execute(HttpContext context, CancellationToken cancellationToken) =>
        await runAsManager.FromMaybe(() => "RunAs not supported").FinishRunAsUserAsync(cancellationToken);
}
