using System.Linq.Expressions;
using Anch.Core;
using Anch.HierarchicalExpand;
using Anch.IdentitySource;
using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public class DirectLevelExtractor<TSecurityContext>(
    IServiceProxyFactory serviceProxyFactory,
    IHierarchicalInfoSource hierarchicalInfoSource,
    IIdentityInfo<TSecurityContext> identityInfo) : IDirectLevelExtractor<TSecurityContext>
{
    private readonly Lazy<IDirectLevelExtractor<TSecurityContext>> lazyInnerService = new(() =>
    {
        var genericExtractorType = hierarchicalInfoSource.IsHierarchical(typeof(TSecurityContext))
            ? typeof(HierarchicalDirectLevelExtractor<,>)
            : typeof(PlainDirectLevelExtractor<,>);

        var extractorType = genericExtractorType.MakeGenericType(typeof(TSecurityContext), identityInfo.IdentityType);

        return serviceProxyFactory.Create<IDirectLevelExtractor<TSecurityContext>>(extractorType);
    });

    public Expression<Func<IQueryable<TSecurityContext>, int>> GetDirectLevelExpression(NotificationFilterGroup notificationFilterGroup) =>
        this.lazyInnerService.Value.GetDirectLevelExpression(notificationFilterGroup);
}

public abstract class DirectLevelExtractor<TSecurityContext, TSecurityContextIdent>(
    INotificationFilterGroupConverter notificationFilterGroupConverter) : IDirectLevelExtractor<TSecurityContext>
    where TSecurityContextIdent : notnull
{
    public Expression<Func<IQueryable<TSecurityContext>, int>> GetDirectLevelExpression(NotificationFilterGroup notificationFilterGroup) =>
        this.GetDirectLevelExpression(notificationFilterGroupConverter.Convert<TSecurityContextIdent>(notificationFilterGroup));

    protected abstract Expression<Func<IQueryable<TSecurityContext>, int>> GetDirectLevelExpression(
        NotificationFilterGroup<TSecurityContextIdent> notificationFilterGroup);
}