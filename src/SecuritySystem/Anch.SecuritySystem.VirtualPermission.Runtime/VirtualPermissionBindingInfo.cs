using System.Collections.Immutable;
using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.IdentitySource;

namespace Anch.SecuritySystem.VirtualPermission;

public abstract record VirtualPermissionBindingInfo
{
    private ImmutableArray<Type>? securityContextTypes;

    public abstract Type PermissionType { get; }

    public ImmutableArray<LambdaExpression> Restrictions { get; init; } = [];

    public ImmutableArray<Type> SecurityContextTypes =>
        this.securityContextTypes ??=
            [
                ..this.Restrictions
                    .Select(restrictionPath => restrictionPath.ReturnType.GetCollectionElementTypeOrSelf())
                    .Distinct()
            ];

    public abstract ImmutableArray<VirtualPermissionSecurityRoleItemBindingInfo> BaseItems { get; }
}

public record VirtualPermissionBindingInfo<TPermission> : VirtualPermissionBindingInfo
{
    private ImmutableArray<VirtualPermissionSecurityRoleItemBindingInfo>? baseItems;

    public override Type PermissionType { get; } = typeof(TPermission);

    public override ImmutableArray<VirtualPermissionSecurityRoleItemBindingInfo> BaseItems => this.baseItems ??= [..this.Items];

    public ImmutableArray<VirtualPermissionSecurityRoleItemBindingInfo<TPermission>> Items { get; init; } = [];

    public Expression<Func<TPermission, Array>> GetRestrictionsArrayExpr(LambdaExpression? filter, IdentityInfo identityInfo)
	{
		return new Func<Expression<Func<ISecurityContext, bool>>?, IIdentityInfo<ISecurityContext, Ignore>, Expression<Func<TPermission, Array>>>(
				this.GetRestrictionsArrayExpr)
			.CreateGenericMethod(identityInfo.DomainObjectType, identityInfo.IdentityType)
			.Invoke<Expression<Func<TPermission, Array>>>(this, filter, identityInfo);
	}

    public Expression<Func<TPermission, Array>> GetRestrictionsArrayExpr<TSecurityContext, TSecurityContextIdent>(
        Expression<Func<TSecurityContext, bool>>? filter,
        IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
        where TSecurityContext : ISecurityContext
        where TSecurityContextIdent : notnull
    {
        return

            from idents in this.GetRestrictionsExpr(filter, identityInfo.Id.Path)

            select (Array)idents.ToArray();
    }

    public Expression<Func<TPermission, IEnumerable<TResult>>> GetRestrictionsExpr<TSecurityContext, TResult>(
        Expression<Func<TSecurityContext, bool>>? filter,
        Expression<Func<TSecurityContext, TResult>> selector)
		where TSecurityContext : ISecurityContext
	{
		var expressions = this.GetManyRestrictionsExpr(filter, selector);

		return expressions.Match(
			() => _ => Array.Empty<TResult>(),
			single => single,
			many => many.Aggregate((state, expr) =>
				from ids1 in state
				from ide2 in expr
				select ids1.Concat(ide2)));
	}

    private IEnumerable<Expression<Func<TPermission, IEnumerable<TResult>>>> GetManyRestrictionsExpr<TSecurityContext, TResult>(
        Expression<Func<TSecurityContext, bool>>? filter,
        Expression<Func<TSecurityContext, TResult>> selector)
        where TSecurityContext : ISecurityContext
    {
        foreach (var restrictionPath in this.Restrictions)
        {
            if (restrictionPath is Expression<Func<TPermission, TSecurityContext?>> singlePath)
            {
                yield return ExpressionEvaluateHelper.InlineEvaluate(ee =>
                {
                    if (filter == null)
                    {
                        return singlePath.Select(IEnumerable<TResult> (securityContext) =>
                            securityContext != null ? new[] { ee.Evaluate(selector, securityContext) } : Array.Empty<TResult>());
                    }
                    else
                    {
                        return singlePath.Select(IEnumerable<TResult> (securityContext) =>
                            securityContext != null && ee.Evaluate(filter, securityContext)
                                ? new[] { ee.Evaluate(selector, securityContext) }
                                : Array.Empty<TResult>());
                    }
                });
            }
            else if (restrictionPath is Expression<Func<TPermission, IEnumerable<TSecurityContext>>> manyPath)
            {
                yield return ExpressionEvaluateHelper.InlineEvaluate(ee =>
                {
                    if (filter == null)
                    {
                        return manyPath.Select(securityContexts =>
                            securityContexts.Select(securityContext => ee.Evaluate(selector, securityContext)));
                    }
                    else
                    {
                        return manyPath.Select(securityContexts => securityContexts
                            .Where(securityContext => ee.Evaluate(filter, securityContext))
                            .Select(securityContext => ee.Evaluate(selector, securityContext)));
                    }
                });
            }
        }
    }
}