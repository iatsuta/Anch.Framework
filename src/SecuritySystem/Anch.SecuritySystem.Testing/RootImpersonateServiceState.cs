using System.Collections.Concurrent;

using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Testing;

public record RootImpersonateServiceState : ImpersonateState
{
    public ConcurrentDictionary<UserCredential, string> Cache { get; set; } = [];

    public void Reset()
    {
        this.CustomUserCredential = null;
        this.Cache = [];
    }
}