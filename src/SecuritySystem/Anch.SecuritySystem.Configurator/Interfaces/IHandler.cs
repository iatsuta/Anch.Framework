using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Interfaces;

/// <summary>A handler of a configurator REST request.</summary>
public interface IHandler
{
    Task Execute(HttpContext context, CancellationToken ct);
}
