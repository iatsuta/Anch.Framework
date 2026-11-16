namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>The principal without its permissions.</summary>
/// <param name="Identity">The principal identifier.</param>
/// <param name="Name">The principal name.</param>
/// <param name="IsVirtual">Whether the principal is computed rather than stored.</param>
public record ManagedPrincipalHeader(SecurityIdentity Identity, string Name, bool IsVirtual);