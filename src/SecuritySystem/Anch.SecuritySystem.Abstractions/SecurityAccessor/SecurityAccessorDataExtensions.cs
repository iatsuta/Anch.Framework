using Anch.Core;

namespace Anch.SecuritySystem.SecurityAccessor;

/// <summary>Extensions for <see cref="SecurityAccessorData"/>.</summary>
public static class SecurityAccessorDataExtensions
{
    /// <summary>Combines the descriptions: the result allows what any of them allows; an empty sequence gives <see cref="SecurityAccessorData.Empty"/>.</summary>
    /// <param name="source">The descriptions to combine.</param>
    public static SecurityAccessorData Or(this IEnumerable<SecurityAccessorData> source) =>
        source.Match(
            () => SecurityAccessorData.Empty,
            result => result,
            results => results.Aggregate(
                SecurityAccessorData.Empty,
                (v1, v2) => new SecurityAccessorData.OrSecurityAccessorData(v1, v2)));
}
