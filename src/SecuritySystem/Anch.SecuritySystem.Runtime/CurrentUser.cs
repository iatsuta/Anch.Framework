using Anch.Core.Auth;
using Anch.SecuritySystem.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem;

public class CurrentUser([FromKeyedServices(ICurrentUser.ImpersonatedKey)] ICurrentUser impersonatedCurrentUser, IRunAsManager? runAsManager = null)
    : ICurrentUser
{
    public string Name => runAsManager?.RunAsUser?.Name ?? impersonatedCurrentUser.Name;
}