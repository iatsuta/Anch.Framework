using System.Linq.Expressions;

using CommonFramework;
using CommonFramework.ExpressionEvaluate;
using CommonFramework.IdentitySource;
using CommonFramework.RelativePath;
using CommonFramework.VisualIdentitySource;

using Microsoft.Extensions.DependencyInjection;
using SecuritySystem.Providers;
using SecuritySystem.SecurityAccessor;
using SecuritySystem.Services;

namespace SecuritySystem.UserSource;

public class CurrentUserSecurityProvider<TDomainObject>(
    IServiceProvider serviceProvider,
	IServiceProxyFactory serviceProxyFactory,
    IEnumerable<UserSourceInfo> userSourceInfoList,
    IIdentityInfoSource identityInfoSource,
    SecurityRuleCredential? securityRuleCredential = null,
    CurrentUserSecurityProviderRelativeKey? key = null) : ISecurityProvider<TDomainObject>
{
    private readonly Lazy<ISecurityProvider<TDomainObject>> lazyInnerService = new(() =>
    {
	    var (actualUserSourceInfo, actualRelativeDomainPathInfo) =
		    TryGetActualUserSourceInfo() ?? throw new SecuritySystemException($"Can't found RelativePath for {typeof(TDomainObject)}");

	    var userIdentityInfo = identityInfoSource.GetIdentityInfo(actualUserSourceInfo.UserType);

        var innerServiceType =
            typeof(CurrentUserSecurityProvider<,,>).MakeGenericType(typeof(TDomainObject), actualUserSourceInfo.UserType, userIdentityInfo.IdentityType);

        var innerServiceArgs = new[]
            {
                actualRelativeDomainPathInfo,
                securityRuleCredential
            }.Where(arg => arg != null)
            .Select(arg => arg!)
            .ToArray();

        return serviceProxyFactory.Create<ISecurityProvider<TDomainObject>>(innerServiceType, innerServiceArgs);

		(UserSourceInfo, object)? TryGetActualUserSourceInfo()
		{
			foreach (var userSourceInfo in userSourceInfoList)
			{
				var relativeDomainPathInfoType = typeof(IRelativeDomainPathInfo<,>).MakeGenericType(typeof(TDomainObject), userSourceInfo.UserType);

				var relativePathKey = key?.Name;

				var relativeDomainPathInfo = relativePathKey == null
					? serviceProvider.GetService(relativeDomainPathInfoType)
					: serviceProvider.GetKeyedService(relativeDomainPathInfoType, relativePathKey);

				if (relativeDomainPathInfo != null)
				{
					return (userSourceInfo, relativeDomainPathInfo);
				}
			}

			return null;
		}
	});

	private ISecurityProvider<TDomainObject> InnerService => this.lazyInnerService.Value;

    public IQueryable<TDomainObject> Inject(IQueryable<TDomainObject> queryable) => this.InnerService.Inject(queryable);

    public ValueTask<AccessResult> GetAccessResultAsync(TDomainObject domainObject, CancellationToken cancellationToken) => this.InnerService.GetAccessResultAsync(domainObject, cancellationToken);

    public ValueTask<bool> HasAccessAsync(TDomainObject domainObject, CancellationToken cancellationToken) => this.InnerService.HasAccessAsync(domainObject, cancellationToken);

    public ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken) => this.InnerService.GetAccessorDataAsync(domainObject, cancellationToken);
}

public class CurrentUserSecurityProvider<TDomainObject, TUser, TIdent>(
    IExpressionEvaluatorStorage expressionEvaluatorStorage,
    IRelativeDomainPathInfo<TDomainObject, TUser> relativeDomainPathInfo,
    IIdentityInfo<TUser, TIdent> identityInfo,
    IVisualIdentityInfo<TUser> visualIdentityInfo,
    ISecurityIdentityConverter<TIdent> securityIdentityConverter,
    IUserNameResolver<TUser> userNameResolver,
    IUserSource<TUser> userSource,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null,
    SecurityRuleCredential? baseSecurityRuleCredential = null) : SecurityProvider<TDomainObject>(expressionEvaluatorStorage)
    where TUser : class
    where TIdent : notnull
{
    public override Expression<Func<TDomainObject, bool>> SecurityFilter { get; } =

        defaultCancellationTokenSource.RunSync<Expression<Func<TUser, bool>>>(async ct =>
            {
                var securityRuleCredential = baseSecurityRuleCredential ?? new SecurityRuleCredential.CurrentUserWithRunAsCredential();

                var userName = await userNameResolver.GetUserNameAsync(securityRuleCredential, ct);

                if (userName == null)
                {
                    return _ => true;
                }
                else
                {
                    var userId = (await userSource.ToSimple().GetUserAsync(userName, ct))
                        .Identity
                        .Pipe(securityIdentityConverter.Convert)
                        .Id;

                    return identityInfo.Id.Path.Select(ExpressionHelper.GetEqualityWithExpr(userId));
                }
            })
            .Pipe(relativeDomainPathInfo.CreateCondition);

    public override async ValueTask<SecurityAccessorData> GetAccessorDataAsync(TDomainObject domainObject, CancellationToken cancellationToken)
    {
        var users = await relativeDomainPathInfo
            .GetRelativeObjects(domainObject)
            .ToAsyncEnumerable()
            .Select(visualIdentityInfo.Name.Getter)
            .ToImmutableArrayAsync(cancellationToken);

        return SecurityAccessorData.Return(users);
    }
}