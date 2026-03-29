using CommonFramework.Auth;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.Services;

namespace SecuritySystem;

public class CurrentUser([FromKeyedServices(ICurrentUser.ImpersonatedKey)] ICurrentUser impersonatedCurrentUser, IRunAsManager? runAsManager = null)
    : ICurrentUser
{
    public string Name => runAsManager?.RunAsUser?.Name ?? impersonatedCurrentUser.Name;
}