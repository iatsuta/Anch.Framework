using System.Text.Json;

using Anch.SecuritySystem.Configurator.Interfaces;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public abstract class BaseReadHandler : IHandler
{
    public async Task Execute(HttpContext context, CancellationToken ct)
    {
        var data = await this.GetDataAsync(context, ct);
        await context.Response.WriteAsync(JsonSerializer.Serialize(data), ct);
    }

    protected abstract Task<object> GetDataAsync(HttpContext context, CancellationToken ct);
}
