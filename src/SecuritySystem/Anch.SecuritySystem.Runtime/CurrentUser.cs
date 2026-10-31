using Anch.Core.Auth;
using Anch.SecuritySystem.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem;

/// <summary>The effective current user: the impersonated user, or the user being run as when a RunAs session is active.</summary>
public class CurrentUser([FromKeyedServices(ICurrentUser.ImpersonatedKey)] ICurrentUser impersonatedCurrentUser, IRunAsManager? runAsManager = null)
    : ICurrentUser
{
    public string Name => runAsManager?.RunAsUser?.Name ?? impersonatedCurrentUser.Name;
}