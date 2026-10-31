// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>
/// Checks access that does not depend on a particular domain object: "may the current user act as an administrator?",
/// "may the current user edit orders?". For per-object checks and query filtering use <see cref="DomainServices.IDomainSecurityService{TDomainObject}"/>.
/// </summary>
/// <example>
/// <code>
/// if (!await securitySystem.HasAccessAsync(SecurityRole.Administrator, ct)) { ... }
/// await securitySystem.CheckAccessAsync(MyRoles.Manager, ct);
/// </code>
/// </example>
public interface ISecuritySystem
{
    /// <summary>Determines whether the rule is satisfied by any permission of the user.</summary>
    /// <param name="securityRule">The required access.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<bool> HasAccessAsync(DomainSecurityRule securityRule, CancellationToken ct);

    /// <summary>Throws the exception produced by <c>IAccessDeniedExceptionService</c> when the rule is not satisfied.</summary>
    /// <param name="securityRule">The required access.</param>
    /// <param name="ct">Cancellation token.</param>
    Task CheckAccessAsync(DomainSecurityRule securityRule, CancellationToken ct);
}