using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Interfaces;

public interface IHandler
{
    Task Execute(HttpContext context, CancellationToken cancellationToken);
}
