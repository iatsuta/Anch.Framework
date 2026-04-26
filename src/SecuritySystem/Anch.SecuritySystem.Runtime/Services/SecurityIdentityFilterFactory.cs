using System.Linq.Expressions;

using Anch.Core;
using Anch.IdentitySource;

namespace Anch.SecuritySystem.Services;

public class SecurityIdentityFilterFactory<TDomainObject>(IServiceProxyFactory serviceProxyFactory, IIdentityInfo<TDomainObject> identityInfo)
	: ISecurityIdentityFilterFactory<TDomainObject>
{
	private readonly Lazy<ISecurityIdentityFilterFactory<TDomainObject>> lazyInnerService = new(() =>
	{
		var innerServiceType = typeof(SecurityIdentityFilterFactory<,>).MakeGenericType(typeof(TDomainObject), identityInfo.IdentityType);

		return  serviceProxyFactory.Create<ISecurityIdentityFilterFactory<TDomainObject>>(innerServiceType);
	});

	public Expression<Func<TDomainObject, bool>> CreateFilter(SecurityIdentity securityIdentity) =>
		this.lazyInnerService.Value.CreateFilter(securityIdentity);
}

public class SecurityIdentityFilterFactory<TDomainObject, TIdent>(
	IIdentityInfo<TDomainObject, TIdent> identityInfo,
	ISecurityIdentityConverter<TIdent> securityIdentityConverter) : ISecurityIdentityFilterFactory<TDomainObject>
	where TDomainObject : class
	where TIdent : notnull
{
	public Expression<Func<TDomainObject, bool>> CreateFilter(SecurityIdentity securityIdentity)
	{
		return identityInfo.Id.Path.Select(ExpressionHelper.GetEqualityWithExpr(securityIdentityConverter.Convert(securityIdentity).Id));
	}
}