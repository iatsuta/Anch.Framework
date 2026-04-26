using System.Collections.Immutable;

using Anch.Core;
using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.SecuritySystem.Notification.Domain;

namespace Anch.SecuritySystem.Notification;

public class NotificationPermissionExtractor<TPermission>(
    INotificationPermissionFilterFactory<TPermission> notificationPermissionFilterFactory,
    IServiceProxyFactory serviceProxyFactory,
    IQueryableSource queryableSource) : INotificationPermissionExtractor<TPermission>
    where TPermission : class
{
    private const string LevelsSeparator = "|";

    private const string LevelValueSeparator = ":";

    public IAsyncEnumerable<TPermission> GetPermissionsAsync(
        ImmutableArray<SecurityRole> securityRoles,
        ImmutableArray<NotificationFilterGroup> notificationFilterGroups)
    {
        var startPermissionQ = queryableSource.GetQueryable<TPermission>()
            .Where(notificationPermissionFilterFactory.Create(securityRoles))
            .Select(p => new PermissionLevelInfo<TPermission> { Permission = p, LevelInfo = "" });

        var typeArr = notificationFilterGroups.Select(g => g.GetSecurityContextType()).ToArray();

        var permissionInfoResult = notificationFilterGroups
            .Aggregate(startPermissionQ, (q, g) => this.ApplyNotificationFilter(q, g, typeArr.IndexOf(g.GetSecurityContextType()))).GenericAsAsyncEnumerable();

        var parsedLevelInfoResult =
            permissionInfoResult.Select(principalInfo => new
                {
                    principalInfo.Permission,
                    LevelDict = principalInfo.LevelInfo
                        .Split(LevelsSeparator, StringSplitOptions.RemoveEmptyEntries)
                        .Select<string, string[]>(levelData => levelData.Split(LevelValueSeparator))
                        .ToDictionary(
                            levelParts => typeArr[int.Parse(levelParts[0])],
                            levelParts => int.Parse(levelParts[1]))
                });

        var optimalRequest = notificationFilterGroups.Aggregate(parsedLevelInfoResult, (state, notificationFilterGroup) =>
        {
            if (notificationFilterGroup.ExpandType == NotificationExpandType.All)
            {
                return state;
            }
            else
            {
                var request =

                    from pair in state

                    group pair by pair.LevelDict[notificationFilterGroup.GetSecurityContextType()]

                    into levelGroup

                    orderby levelGroup.Key descending

                    select levelGroup;

                return request.Take(1).SelectMany(v => v);
            }
        });

        return optimalRequest.Select(pair => pair.Permission).Distinct<TPermission>();
    }

    private IQueryable<PermissionLevelInfo<TPermission>> ApplyNotificationFilter(
        IQueryable<PermissionLevelInfo<TPermission>> source,
        NotificationFilterGroup notificationFilterGroup,
        int securityContextIndex)
    {
        var extractorType = typeof(PermissionLevelInfoExtractor<,>).MakeGenericType(typeof(TPermission), notificationFilterGroup.GetSecurityContextType());

        var selector = serviceProxyFactory.Create<IPermissionLevelInfoExtractor<TPermission>>(extractorType).GetSelector(notificationFilterGroup);

        return

            from permissionLevelInfo in source.Select(selector)

            where permissionLevelInfo.Level != PriorityLevels.AccessDenied

            select new PermissionLevelInfo<TPermission>
            {
                Permission = permissionLevelInfo.Permission,
                LevelInfo = permissionLevelInfo.LevelInfo
                            + $"{LevelsSeparator}{securityContextIndex}{LevelValueSeparator}{permissionLevelInfo.Level}"
            };
    }
}