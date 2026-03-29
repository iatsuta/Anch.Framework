using CommonFramework.Auth;

using Microsoft.Extensions.DependencyInjection;

namespace SecuritySystem.Services;

public class RawCurrentUser([FromKeyedServices(ICurrentUser.DefaultKey)]ICurrentUser defaultCurrentUser) : ICurrentUser
{
    public string Name => defaultCurrentUser.Name;
}