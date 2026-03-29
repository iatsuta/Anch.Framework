using System.Linq.Expressions;

using CommonFramework;
using CommonFramework.ExpressionEvaluate;
using SecuritySystem.Notification.Domain;
using SecuritySystem.Services;

namespace SecuritySystem.Notification;

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

        return ExpressionEvaluateHelper.InlineEvaluate(ee =>

            from permissionInfo in ExpressionHelper.GetIdentity<PermissionLevelInfo<TPermission>>()

            let permission = permissionInfo.Permission

            let permissionSecurityContextItems = ee.Evaluate(restrictionsPath, permission)

            let directLevel = ee.Evaluate(getDirectLevelExpression, permissionSecurityContextItems)

            let unrestrictedLevel = unrestrictedAccess && ee.Evaluate(unrestrictedFilter, permission) ? PriorityLevels.Unrestricted : PriorityLevels.AccessDenied

            let resultLevel = directLevel > unrestrictedLevel ? directLevel : unrestrictedLevel

            select new FullPermissionLevelInfo<TPermission> { Permission = permissionInfo.Permission, LevelInfo = permissionInfo.LevelInfo, Level = resultLevel });
    }
}