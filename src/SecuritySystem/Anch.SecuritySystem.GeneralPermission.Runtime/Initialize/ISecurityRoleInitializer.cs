using Anch.Core;

namespace Anch.SecuritySystem.GeneralPermission.Initialize;

/// <summary>Synchronizes the stored role entities with the registered roles.</summary>
public interface ISecurityRoleInitializer<TSecurityRole> : ISecurityRoleInitializer
{
    Task<MergeResult<TSecurityRole, FullSecurityRole>> Initialize(IEnumerable<FullSecurityRole> securityRoles, CancellationToken ct);

    new Task<MergeResult<TSecurityRole, FullSecurityRole>> Initialize(CancellationToken ct);
}

/// <summary>Synchronizes the stored role entities with the registered roles.</summary>
public interface ISecurityRoleInitializer : IInitializer;