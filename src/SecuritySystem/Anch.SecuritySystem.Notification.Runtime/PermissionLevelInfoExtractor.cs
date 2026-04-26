using System.Linq.Expressions;
using Anch.Core;
using Anch.Core.ExpressionEvaluate;
using Anch.SecuritySystem.Notification.Domain;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Notification;

public class PermissionLevelInfoExtractor<TPermission, TSecurityContext>(
    IPermissionTypedRestrictionBindingInfo<TPermission> permissionRestrictionBinding,
    IDirectLevelExtractor<TSecurityContext> directLevelExtractor) : IPermissionLevelInfoExtractor<TPermission>
    where TSecurityContext : class, ISecurityContext
{
    public Expression<Func<PermissionLevelInfo<TPermission>, FullPermissionLevelInfo<TPermission>>> GetSelector(NotificationFilterGroup notificationFilterGroup)
    {
        var unrestrictedAccess = notificationFilterGroup.ExpandType.AllowEmpty();

        var restrictionsPath = permissionRestrictionBinding.GetRestrictionsPath<TSecurityContext>();
        var unrestrictedFilter = permissionRestrictionBinding.GetUnrestrictedFilter<TSecurityContext>();

        var getDirectLevelExpression = directLevelExtractor.GetDirectLevelExpression(notificationFilterGroup);

        var getPermissionDirectLevel = restrictionsPath.Select(getDirectLevelExpression);

        var getUnrestrictedLevel = unrestrictedAccess
            ? unrestrictedFilter.Select(v => v ? PriorityLevels.Unrestricted : PriorityLevels.AccessDenied)
            : _ => PriorityLevels.AccessDenied;

        return ExpressionEvaluateHelper.InlineEvaluate(ee =>

            from permissionInfo in ExpressionHelper.GetIdentity<PermissionLevelInfo<TPermission>>()

            let permission = permissionInfo.Permission

            let directLevel = ee.Evaluate(getPermissionDirectLevel, permission)

            let unrestrictedLevel = ee.Evaluate(getUnrestrictedLevel, permission)

            let resultLevel = directLevel > unrestrictedLevel ? directLevel : unrestrictedLevel

            select new FullPermissionLevelInfo<TPermission> { Permission = permissionInfo.Permission, LevelInfo = permissionInfo.LevelInfo, Level = resultLevel });
    }
}