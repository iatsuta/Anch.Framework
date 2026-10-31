using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.HierarchicalExpand;
using Anch.IdentitySource;
using Anch.SecuritySystem.ExternalSystem;

namespace Anch.SecuritySystem.Builders.AccessorsBuilder;

/// <summary>Computes who has access to an object (the accessors), for a path to a collection of security context objects.</summary>
public class ManyContextFilterBuilder<TDomainObject, TPermission, TSecurityContext, TSecurityContextIdent>(
    IExpressionEvaluatorStorage expressionEvaluatorStorage,
    IPermissionSystem<TPermission> permissionSystem,
    IHierarchicalObjectExpanderFactory hierarchicalObjectExpanderFactory,
    SecurityPath<TDomainObject>.ManySecurityPath<TSecurityContext> securityPath,
    SecurityContextRestriction<TSecurityContext>? securityContextRestriction,
    IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
    : ByIdentsFilterBuilder<TDomainObject, TPermission, TSecurityContext, TSecurityContextIdent>(permissionSystem, hierarchicalObjectExpanderFactory, securityPath, securityContextRestriction, identityInfo)
    where TSecurityContext : class, ISecurityContext
    where TSecurityContextIdent : notnull
{
    private readonly IExpressionEvaluator expressionEvaluator =
        expressionEvaluatorStorage.GetForType(typeof(ManyContextFilterBuilder<TDomainObject, TPermission, TSecurityContext, TSecurityContextIdent>));

    protected override IEnumerable<TSecurityContext> GetSecurityObjects(TDomainObject domainObject) =>

        this.expressionEvaluator.Evaluate(securityPath.Expression, domainObject).EmptyIfNull();
}