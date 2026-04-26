using Anch.SecuritySystem.Configurator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Anch.SecuritySystem.Configurator.Handlers;

public class GetModulesHandler(IEnumerable<IConfiguratorModule> modules) : BaseReadHandler, IGetModulesHandler
{
    protected override async Task<object> GetDataAsync(HttpContext context, CancellationToken cancellationToken)
    {
        return modules.Select(module => module.Name).ToList();
    }
}
