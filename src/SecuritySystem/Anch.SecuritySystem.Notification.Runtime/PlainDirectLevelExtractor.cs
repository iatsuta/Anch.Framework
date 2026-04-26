using System.Linq.Expressions;

using Anch.IdentitySource;
using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public class PlainDirectLevelExtractor<TSecurityContext, TSecurityContextIdent>(
    INotificationFilterGroupConverter notificationFilterGroupConverter,
    IIdentityInfo<TSecurityContext, TSecurityContextIdent> identityInfo)
    : DirectLevelExtractor<TSecurityContext, TSecurityContextIdent>(notificationFilterGroupConverter)
    where TSecurityContextIdent : notnull
{
    protected override Expression<Func<IQueryable<TSecurityContext>, int>> GetDirectLevelExpression(
        NotificationFilterGroup<TSecurityContextIdent> notificationFilterGroup)
    {
        var containsFilter = identityInfo.CreateFilter(notificationFilterGroup.Idents);

        return permissionSecurityContextItems => permissionSecurityContextItems.Any(containsFilter) ? 0 : PriorityLevels.AccessDenied;
    }
}