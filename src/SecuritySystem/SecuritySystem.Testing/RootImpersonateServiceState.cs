using SecuritySystem.Services;

using System.Collections.Concurrent;

namespace SecuritySystem.Testing;

public record RootImpersonateServiceState : ImpersonateState
{
    public ConcurrentDictionary<UserCredential, string> Cache { get; set; } = [];

    public void Reset()
    {
        this.CustomUserCredential = null;
        this.Cache = [];
    }
}