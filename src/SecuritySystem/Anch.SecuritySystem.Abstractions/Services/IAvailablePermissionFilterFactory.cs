using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

/// <summary>Builds the filter that selects the permissions satisfying a rule.</summary>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
public interface IAvailablePermissionFilterFactory<TPermission>
{
    /// <summary>Builds the filter.</summary>
    /// <param name="securityRule">The rule the permissions must satisfy.</param>
    Expression<Func<TPermission, bool>> CreateFilter(DomainSecurityRule.RoleBaseSecurityRule securityRule);
}