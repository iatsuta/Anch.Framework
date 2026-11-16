using System.Linq.Expressions;

using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

/// <summary>Finds the contexts that a permission may name directly for an event.</summary>
public interface IDirectLevelExtractor<TSecurityContext>
{
    Expression<Func<IQueryable<TSecurityContext>, int>> GetDirectLevelExpression(NotificationFilterGroup notificationFilterGroup);
}