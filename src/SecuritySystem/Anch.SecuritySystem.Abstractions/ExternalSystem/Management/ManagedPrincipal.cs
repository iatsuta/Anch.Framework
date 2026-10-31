using System.Collections.Immutable;

namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>A principal together with its permissions.</summary>
/// <param name="Header">The principal itself.</param>
/// <param name="Permissions">The permissions of the principal.</param>
public record ManagedPrincipal(ManagedPrincipalHeader Header, ImmutableArray<ManagedPermission> Permissions);
