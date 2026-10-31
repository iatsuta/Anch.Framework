using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem.Services;

/// <summary>Describes how the restrictions of a permission are stored for one security context type.</summary>
public interface IPermissionTypedRestrictionBindingInfo<TPermission>
{
    Expression<Func<TPermission, IQueryable<TSecurityContext>>> GetRestrictionsPath<TSecurityContext>()
        where TSecurityContext : class, ISecurityContext;

    Expression<Func<TPermission, bool>> GetUnrestrictedFilter<TSecurityContext>()
        where TSecurityContext : class, ISecurityContext => this.GetRestrictionsPath<TSecurityContext>().Select(v => !v.Any());
}