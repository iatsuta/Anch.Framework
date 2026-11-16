using Anch.Core;

namespace Anch.SecuritySystem.Providers;

/// <summary>Extensions for <see cref="AccessResult"/>.</summary>
public static class AccessResultExtensions
{
    /// <summary>Sets the denied object on a denial; a grant is returned unchanged.</summary>
    /// <param name="accessResult">The result.</param>
    /// <param name="domainObject">The object to report.</param>
    /// <typeparam name="TDomainObject">The type of the object.</typeparam>
    public static AccessResult TryOverrideDomainObject<TDomainObject>(this AccessResult accessResult, TDomainObject domainObject) =>
        accessResult.TryOverrideAccessDeniedResult(
            accessDeniedResult => accessDeniedResult with { DomainObjectInfo = (domainObject, typeof(TDomainObject))! });

    /// <summary>Rewrites a denial; a grant is returned unchanged.</summary>
    /// <param name="accessResult">The result.</param>
    /// <param name="selector">Rewrites the denial.</param>
    public static AccessResult TryOverrideAccessDeniedResult(
        this AccessResult accessResult,
        Func<AccessResult.AccessDeniedResult, AccessResult.AccessDeniedResult> selector) =>
        accessResult switch
        {
            AccessResult.AccessDeniedResult accessDeniedResult => selector(accessDeniedResult),
            _ => accessResult
        };

    /// <summary>Combines the results: granted if any is granted; an empty sequence is denied.</summary>
    /// <param name="source">The results to combine.</param>
    public static AccessResult Or(this IEnumerable<AccessResult> source) =>
        source.Match(
            () => AccessResult.AccessDeniedResult.Default,
            result => result,
            results => results.Aggregate((AccessResult)AccessResult.AccessDeniedResult.Default, (v1, v2) => v1.Or(v2)));
}
