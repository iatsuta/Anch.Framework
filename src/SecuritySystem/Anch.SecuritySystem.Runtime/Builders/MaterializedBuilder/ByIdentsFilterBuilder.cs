using System.Collections;
using System.Linq.Expressions;

namespace Anch.SecuritySystem.Builders.MaterializedBuilder;

/// <summary>Builds the access filter with the permissions of the user loaded into memory first, for a path that matches security context identifiers against the permission restrictions.</summary>
public abstract class ByIdentsFilterBuilder<TDomainObject, TSecurityContext, TSecurityContextIdent>(SecurityContextRestriction<TSecurityContext>? securityContextRestriction)
    : SecurityFilterBuilder<TDomainObject>
    where TSecurityContext : class, ISecurityContext
{
    public sealed override Expression<Func<TDomainObject, bool>> GetSecurityFilterExpression(IReadOnlyDictionary<Type, IEnumerable> permission)
    {
        if (permission.TryGetValue(typeof(TSecurityContext), out var securityIdents))
        {
            return this.GetSecurityFilterExpression((IEnumerable<TSecurityContextIdent>)securityIdents);
        }
        else
        {
            var allowsUnrestrictedAccess = securityContextRestriction?.Required != true;

            return _ => allowsUnrestrictedAccess;
        }
    }

    protected abstract Expression<Func<TDomainObject, bool>> GetSecurityFilterExpression(IEnumerable<TSecurityContextIdent> permissionIdents);
}