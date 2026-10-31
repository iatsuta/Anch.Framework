using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

/// <summary>Converts a security identifier to a typed one, for example an untyped string to a <see cref="Guid"/>.</summary>
public interface ISecurityIdentityConverter
{
    /// <summary>Converts the identifier.</summary>
    /// <param name="securityIdentity">The identifier.</param>
    /// <returns>The typed identifier, or <c>null</c> when it cannot be converted.</returns>
    TypedSecurityIdentity? TryConvert(SecurityIdentity securityIdentity);

    /// <summary>Converts the identifier; throws when it cannot be converted.</summary>
    /// <param name="securityIdentity">The identifier.</param>
    TypedSecurityIdentity Convert(SecurityIdentity securityIdentity);
}

/// <summary>Converts security identifiers to identifiers of type <typeparamref name="TIdent"/>.</summary>
/// <typeparam name="TIdent">The target identifier type.</typeparam>
public interface ISecurityIdentityConverter<TIdent> : ISecurityIdentityConverter
    where TIdent : notnull
{
    /// <summary>Converts the identifier.</summary>
    /// <param name="securityIdentity">The identifier.</param>
    /// <returns>The typed identifier, or <c>null</c> when it cannot be converted.</returns>
    new TypedSecurityIdentity<TIdent>? TryConvert(SecurityIdentity securityIdentity);

    /// <summary>Converts the identifier; throws when it cannot be converted.</summary>
    /// <param name="securityIdentity">The identifier.</param>
    new TypedSecurityIdentity<TIdent> Convert(SecurityIdentity securityIdentity);

    /// <summary>Returns an expression that converts identifiers of another type, so it can be used inside queries.</summary>
    /// <typeparam name="TSourceIdent">The source identifier type.</typeparam>
    Expression<Func<TSourceIdent, TIdent>> GetConvertExpression<TSourceIdent>();
}