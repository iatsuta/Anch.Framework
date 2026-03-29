using System.Linq.Expressions;
using SecuritySystem.Notification.Domain;

namespace SecuritySystem.Notification;

public interface IDirectLevelExtractor<TSecurityContext>
{
    Expression<Func<IQueryable<TSecurityContext>, int>> GetDirectLevelExpression(NotificationFilterGroup notificationFilterGroup);
}