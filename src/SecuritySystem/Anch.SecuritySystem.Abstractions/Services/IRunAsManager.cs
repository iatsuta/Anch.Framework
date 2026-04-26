using Anch.SecuritySystem.UserSource;

namespace Anch.SecuritySystem.Services;

public interface IRunAsManager
{
    User? RunAsUser { get; }

    Task StartRunAsUserAsync(UserCredential userCredential, CancellationToken cancellationToken = default);

    Task FinishRunAsUserAsync(CancellationToken cancellationToken = default);
}
