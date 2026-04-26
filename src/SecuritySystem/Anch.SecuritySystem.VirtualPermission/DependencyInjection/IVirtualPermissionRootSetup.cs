using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem.VirtualPermission.DependencyInjection;

public interface IVirtualPermissionRootSetup<TPermission>
    where TPermission : notnull
{
    IVirtualPermissionRootSetup<TPermission> AddRestriction<TSecurityContext>(
        Expression<Func<TPermission, IEnumerable<TSecurityContext>>> path)
        where TSecurityContext : ISecurityContext;

    IVirtualPermissionRootSetup<TPermission> AddRestriction<TSecurityContext>(
        Expression<Func<TPermission, TSecurityContext?>> path)
        where TSecurityContext : ISecurityContext;

    IVirtualPermissionRootSetup<TPermission> SetPeriod(
        PropertyAccessors<TPermission, DateTime?>? startDatePropertyAccessor,
        PropertyAccessors<TPermission, DateTime?>? endDatePropertyAccessor);

    IVirtualPermissionRootSetup<TPermission> SetPeriod(
        Expression<Func<TPermission, DateTime?>>? startDatePath,
        Expression<Func<TPermission, DateTime?>>? endDatePath);

    IVirtualPermissionRootSetup<TPermission> SetComment(Expression<Func<TPermission, string>> commentPath);

    IVirtualPermissionRootSetup<TPermission> SetPermissionDelegation(Expression<Func<TPermission, TPermission?>> newDelegatedFromPath);

    IVirtualPermissionRootSetup<TPermission> AddSecurityRole(SecurityRole securityRole, Action<IVirtualBindingInfoSettingsSetup<TPermission>>? init = null);
}