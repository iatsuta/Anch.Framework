using System.Linq.Expressions;

using CommonFramework.IdentitySource;
using SecuritySystem.Notification.Domain;

namespace SecuritySystem.Notification;

public class PlainDirectLevelExtractor<TSecurityContext, TSecurityContextIdent>(
    IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
    : DirectLevelExtractor<TSecurityContext, TSecurityContextIdent>
    where TSecurityContextIdent : notnull
{
    protected override Expression<Func<IQueryable<TSecurityContext>, int>> GetDirectLevelExpression(
        NotificationFilterGroup<TSecurityContextIdent> notificationFilterGroup)
    {
        var containsFilter = identityInfo.CreateContainsFilter(notificationFilterGroup.Idents);

        return permissionSecurityContextItems => permissionSecurityContextItems.Any(containsFilter) ? 0 : PriorityLevels.AccessDenied;
    }
}