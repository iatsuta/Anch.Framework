using System.Collections.Immutable;

namespace Anch.SecuritySystem.ExternalSystem.Management;

public record ManagedPrincipal(ManagedPrincipalHeader Header, ImmutableArray<ManagedPermission> Permissions);
