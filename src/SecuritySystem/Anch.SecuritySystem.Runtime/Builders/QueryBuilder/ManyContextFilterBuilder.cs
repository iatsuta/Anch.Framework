using System.Linq.Expressions;
using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.HierarchicalExpand;
using Anch.IdentitySource;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.Builders.QueryBuilder;

public class ManyContextFilterBuilder<TDomainObject, TPermission, TSecurityContext, TSecurityContextIdent>(
    IPermissionSystem<TPermission> permissionSystem,
    IHierarchicalObjectExpanderFactory hierarchicalObjectExpanderFactory,
    SecurityPath<TDomainObject>.ManySecurityPath<TSecurityContext> securityPath,
    SecurityContextRestriction<TSecurityContext>? securityContextRestriction,
    IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
    : SecurityFilterBuilder<TDomainObject, TPermission>
    where TSecurityContext : class, ISecurityContext
    where TSecurityContextIdent : notnull
{
    private readonly IPermissionRestrictionSource<TPermission, TSecurityContextIdent> permissionRestrictionSource =
        permissionSystem.GetRestrictionSource<TSecurityContext, TSecurityContextIdent>(securityContextRestriction?.Filter);

    public override Expression<Func<TDomainObject, TPermission, bool>> GetSecurityFilterExpression(HierarchicalExpandType expandType)
    {
        var allowsUnrestrictedAccess = securityContextRestriction?.Required != true;

        var unrestrictedFilter = allowsUnrestrictedAccess
            ? this.permissionRestrictionSource.GetUnrestrictedFilter()
            : _ => false;

        var getIdents = this.permissionRestrictionSource.GetIdentsExpr();

        var expander = hierarchicalObjectExpanderFactory.Create<TSecurityContextIdent>(typeof(TSecurityContext));

        var expandExpression = expander.GetExpandExpression(expandType);

        var expandExpressionQ = getIdents.Select(expandExpression);

        return ExpressionEvaluateHelper.InlineEvaluate<Func<TDomainObject, TPermission, bool>>(ee =>
        {
            if (securityPath.Required)
            {
                return (domainObject, permission) => ee.Evaluate(unrestrictedFilter, permission)

                                                     || ee.Evaluate(securityPath.Expression, domainObject)
                                                         .Any(item => ee.Evaluate(expandExpressionQ, permission).Contains(ee.Evaluate(identityInfo.Id.Path, item)));
            }
            else
            {
                return (domainObject, permission) => ee.Evaluate(unrestrictedFilter, permission)

                                                     || !ee.Evaluate(securityPath.Expression, domainObject).Any()

                                                     || ee.Evaluate(securityPath.Expression, domainObject).Any(item =>
                                                         ee.Evaluate(getIdents, permission).Contains(ee.Evaluate(identityInfo.Id.Path, item)));
            }
        });
    }
}