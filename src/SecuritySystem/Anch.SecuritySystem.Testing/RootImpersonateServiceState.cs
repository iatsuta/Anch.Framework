using System.Collections.Concurrent;

using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Testing;

/// <summary>The impersonation state of the testing root user.</summary>
public record RootImpersonateServiceState : ImpersonateState
{
    public ConcurrentDictionary<UserCredential, string> Cache { get; set; } = [];

    public void Reset()
    {
        this.CustomUserCredential = null;
        this.Cache = [];
    }
}