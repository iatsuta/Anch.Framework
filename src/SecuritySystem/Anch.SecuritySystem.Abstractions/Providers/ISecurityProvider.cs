using Anch.Core;
using Anch.GenericQueryable;
using Anch.SecuritySystem.SecurityAccessor;

namespace Anch.SecuritySystem.Providers;

/// <summary>
/// Provides access checks and filtering for objects.
/// </summary>
/// <typeparam name="TDomainObject">The type of the protected domain objects.</typeparam>
/// <remarks>
/// A provider is obtained from <c>IDomainSecurityService</c> for a rule, or injected as a keyed service with
/// <c>[ViewSecurity]</c>, <c>[EditSecurity]</c> or <c>[DisabledSecurity]</c>. <c>Inject</c> adds the access filter to a query, so it is applied in SQL.
/// </remarks>
public interface ISecurityProvider<TDomainObject> : IQueryableInjector<TDomainObject>
{
    /// <summary>Checks access to the object and describes the result, including the reason when access is denied.</summary>
    /// <param name="domainObject">The object to check.</param>
    /// <param name="ct">Cancellation token.</param>
    async ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken ct) =>
        await this.HasAccessAsync(domainObject, ct)
            ? AccessResult.AccessGrantedResult.Default
            : AccessResult.AccessDeniedResult.Create(domainObject);

    /// <summary>Determines whether the current user may access the object. By default checks whether the filtered one-element query still contains it.</summary>
    /// <param name="domainObject">The object to check.</param>
    /// <param name="ct">Cancellation token.</param>
    async ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken ct) =>
        await this.Inject(new[] { domainObject }.AsQueryable()).GenericContainsAsync(domainObject, ct);

    /// <summary>Describes who has access to the object. The default implementation returns "everyone" or "nobody".</summary>
    /// <param name="domainObject">The object to check.</param>
    /// <param name="ct">Cancellation token.</param>
    async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken ct) =>
        await this.HasAccessAsync(domainObject, ct) ? SecurityAccessorData.Infinity : SecurityAccessorData.Empty;
}