using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem.ExternalSystem;

/// <summary>Reads the restrictions of permissions (the security context objects a permission is limited to).</summary>
public interface IPermissionRestrictionSource<TPermission>
{
    Expression<Func<TPermission, bool>> GetUnrestrictedFilter();
}

/// <summary>Reads the restrictions of permissions (the security context objects a permission is limited to).</summary>
public interface IPermissionRestrictionSource<TPermission, TSecurityContextIdent> : IPermissionRestrictionSource<TPermission>
{
    Expression<Func<TPermission, IEnumerable<TSecurityContextIdent>>> GetIdentsExpr();

    Expression<Func<TPermission, bool>> GetContainsIdentsExpr(IEnumerable<TSecurityContextIdent> idents) => this.GetIdentsExpr()
        .Select(restrictionIdents => restrictionIdents.Any(restrictionIdent => idents.Contains(restrictionIdent)));

    Expression<Func<TPermission, bool>> IPermissionRestrictionSource<TPermission>.GetUnrestrictedFilter()
        => this.GetIdentsExpr().Select(v => !v.Any());
}