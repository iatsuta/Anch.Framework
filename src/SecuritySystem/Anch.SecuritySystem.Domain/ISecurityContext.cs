namespace Anch.SecuritySystem;

/// <summary>
/// Marker interface of a domain type that acts as a typed security context.
/// Permissions are granted on instances of such types (for example, a business unit or a location);
/// register a context with <c>AddSecurityContext&lt;T&gt;</c>.
/// </summary>
public interface ISecurityContext;