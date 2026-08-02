namespace Anch.SecuritySystem.Services;

public interface IRunAsManager
{
    User? RunAsUser { get; }

    Task StartRunAsUserAsync(UserCredential userCredential, CancellationToken ct);

    Task FinishRunAsUserAsync(CancellationToken ct);
}
