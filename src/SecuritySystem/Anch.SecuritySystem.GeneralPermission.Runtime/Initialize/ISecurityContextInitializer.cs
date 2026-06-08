using Anch.Core;

namespace Anch.SecuritySystem.GeneralPermission.Initialize;

public interface ISecurityContextInitializer<TSecurityContextType> : ISecurityContextInitializer
{
    new Task<MergeResult<TSecurityContextType, SecurityContextInfo>> Initialize(CancellationToken ct);
}

public interface ISecurityContextInitializer : IInitializer;