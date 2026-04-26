using System.Linq.Expressions;
using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public interface IDirectLevelExtractor<TSecurityContext>
{
    Expression<Func<IQueryable<TSecurityContext>, int>> GetDirectLevelExpression(NotificationFilterGroup notificationFilterGroup);
}