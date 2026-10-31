namespace Anch.SecuritySystem.Services;

/// <summary>Manages "run as": acting on behalf of another user while keeping the real one. The change is checked by the registered RunAs validators.</summary>
public interface IRunAsManager
{
    /// <summary>The user being run as, or <c>null</c> when there is no RunAs session.</summary>
    User? RunAsUser { get; }

    /// <summary>Starts acting as the given user.</summary>
    /// <param name="userCredential">The user to run as.</param>
    /// <param name="ct">Cancellation token.</param>
    Task StartRunAsUserAsync(UserCredential userCredential, CancellationToken ct);

    /// <summary>Ends the RunAs session.</summary>
    /// <param name="ct">Cancellation token.</param>
    Task FinishRunAsUserAsync(CancellationToken ct);
}
