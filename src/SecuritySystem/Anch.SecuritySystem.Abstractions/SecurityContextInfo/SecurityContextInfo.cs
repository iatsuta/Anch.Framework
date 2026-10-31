// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Registration data of a security context type.</summary>
/// <param name="Identity">Stable identifier of the context type.</param>
/// <param name="Name">Name of the context type.</param>
public abstract record SecurityContextInfo(TypedSecurityIdentity Identity, string Name)
{
    /// <summary>The CLR type of the security context.</summary>
    public abstract Type Type { get; }
}

/// <summary>Registration data of the security context type <typeparamref name="TSecurityContext"/>.</summary>
/// <param name="Identity">Stable identifier of the context type.</param>
/// <param name="Name">Name of the context type.</param>
/// <typeparam name="TSecurityContext">The security context type.</typeparam>
public record SecurityContextInfo<TSecurityContext>(TypedSecurityIdentity Identity, string Name) : SecurityContextInfo(Identity, Name)
    where TSecurityContext : ISecurityContext
{
    public override Type Type { get; } = typeof(TSecurityContext);
}
