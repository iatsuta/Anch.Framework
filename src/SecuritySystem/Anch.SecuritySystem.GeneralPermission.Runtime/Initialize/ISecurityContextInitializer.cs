using Anch.Core;

namespace Anch.SecuritySystem.GeneralPermission.Initialize;

/// <summary>Synchronizes the stored security context type entities with the registered security contexts.</summary>
public interface ISecurityContextInitializer<TSecurityContextType> : ISecurityContextInitializer
{
    new Task<MergeResult<TSecurityContextType, SecurityContextInfo>> Initialize(CancellationToken ct);
}

/// <summary>Synchronizes the stored security context type entities with the registered security contexts.</summary>
public interface ISecurityContextInitializer : IInitializer;