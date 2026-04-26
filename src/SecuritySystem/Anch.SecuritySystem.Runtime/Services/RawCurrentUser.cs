using Anch.Core.Auth;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Services;

public class RawCurrentUser([FromKeyedServices(ICurrentUser.DefaultKey)]ICurrentUser defaultCurrentUser) : ICurrentUser
{
    public string Name => defaultCurrentUser.Name;
}