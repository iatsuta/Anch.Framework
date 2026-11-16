using System.Text.Json;

using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

/// <summary>The base of the handlers that change data; they require the security administrator role.</summary>
public abstract class BaseWriteHandler
{
    // TODO: this can be replaced with built serialization/deserialization
    protected async Task<TModel> ParseRequestBodyAsync<TModel>(HttpContext context)
    {
        using var streamReader = new StreamReader(context.Request.Body);
        var requestBody = await streamReader.ReadToEndAsync(context.RequestAborted);

        return JsonSerializer.Deserialize<TModel>(requestBody)!;
    }
}