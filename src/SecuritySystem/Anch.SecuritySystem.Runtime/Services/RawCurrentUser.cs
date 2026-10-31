using Anch.Core.Auth;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Services;

/// <summary>The current user as the environment reports it, before impersonation and RunAs.</summary>
public class RawCurrentUser([FromKeyedServices(ICurrentUser.DefaultKey)] ICurrentUser defaultCurrentUser) : ICurrentUser
{
    public string Name => defaultCurrentUser.Name;
}