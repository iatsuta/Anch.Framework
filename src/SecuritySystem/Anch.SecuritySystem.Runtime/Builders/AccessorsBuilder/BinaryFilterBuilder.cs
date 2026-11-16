using System.Linq.Expressions;

using Anch.HierarchicalExpand;

namespace Anch.SecuritySystem.Builders.AccessorsBuilder;

/// <summary>Computes who has access to an object (the accessors), for a security path that combines two others.</summary>
public abstract class BinaryFilterBuilder<TDomainObject, TPermission, TSecurityPath>(
    AccessorsFilterBuilderFactory<TDomainObject, TPermission> builderFactory,
    TSecurityPath securityPath,
    IReadOnlyList<SecurityContextRestriction> securityContextRestrictions)
    : AccessorsFilterBuilder<TDomainObject, TPermission>
    where TSecurityPath : SecurityPath<TDomainObject>.BinarySecurityPath
{
    private AccessorsFilterBuilder<TDomainObject, TPermission> LeftBuilder { get; } = builderFactory.CreateBuilder(securityPath.Left, securityContextRestrictions);

    private AccessorsFilterBuilder<TDomainObject, TPermission> RightBuilder { get; } = builderFactory.CreateBuilder(securityPath.Right, securityContextRestrictions);

    protected abstract Expression<Func<TArg, bool>> BuildOperation<TArg>(
        Expression<Func<TArg, bool>> arg1,
        Expression<Func<TArg, bool>> arg2);

    public override Expression<Func<TPermission, bool>> GetAccessorsFilter(TDomainObject domainObject, HierarchicalExpandType expandType)
    {
        var leftFilter = this.LeftBuilder.GetAccessorsFilter(domainObject, expandType);

        var rightFilter = this.RightBuilder.GetAccessorsFilter(domainObject, expandType);

        return this.BuildOperation(leftFilter, rightFilter);
    }
}
