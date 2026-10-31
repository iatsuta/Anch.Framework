using System.Linq.Expressions;

using Anch.Core;

namespace Anch.SecuritySystem.VirtualPermission.DependencyInjection;

/// <summary>Fluent configuration of a virtual permission: which roles an entity grants and which security contexts it is limited to.</summary>
/// <typeparam name="TPermission">The domain entity the permission is computed from.</typeparam>
public interface IVirtualPermissionRootSetup<TPermission>
    where TPermission : notnull
{
    /// <summary>Limits the permission to the security contexts that the entity references through a collection.</summary>
    /// <param name="path">Selects the security contexts.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    IVirtualPermissionRootSetup<TPermission> AddRestriction<TSecurityContext>(
        Expression<Func<TPermission, IEnumerable<TSecurityContext>>> path)
        where TSecurityContext : ISecurityContext;

    /// <summary>Limits the permission to the security context that the entity references.</summary>
    /// <param name="path">Selects the security context.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    IVirtualPermissionRootSetup<TPermission> AddRestriction<TSecurityContext>(
        Expression<Func<TPermission, TSecurityContext?>> path)
        where TSecurityContext : ISecurityContext;

    /// <summary>Sets the validity period properties of the entity.</summary>
    /// <param name="startDatePropertyAccessor">Accessors of the start date; <c>null</c> if there is none.</param>
    /// <param name="endDatePropertyAccessor">Accessors of the end date; <c>null</c> if there is none.</param>
    IVirtualPermissionRootSetup<TPermission> SetPeriod(
        PropertyAccessors<TPermission, DateTime?>? startDatePropertyAccessor,
        PropertyAccessors<TPermission, DateTime?>? endDatePropertyAccessor);

    /// <summary>Sets the validity period properties of the entity.</summary>
    /// <param name="startDatePath">Selects the start date; <c>null</c> if there is none.</param>
    /// <param name="endDatePath">Selects the end date; <c>null</c> if there is none.</param>
    IVirtualPermissionRootSetup<TPermission> SetPeriod(
        Expression<Func<TPermission, DateTime?>>? startDatePath,
        Expression<Func<TPermission, DateTime?>>? endDatePath);

    /// <summary>Sets the property that holds a comment shown with the permission.</summary>
    /// <param name="commentPath">Selects the comment.</param>
    IVirtualPermissionRootSetup<TPermission> SetComment(Expression<Func<TPermission, string>> commentPath);

    /// <summary>Sets the property that references the permission this one was delegated from.</summary>
    /// <param name="newDelegatedFromPath">Selects the source permission.</param>
    IVirtualPermissionRootSetup<TPermission> SetPermissionDelegation(Expression<Func<TPermission, TPermission?>> newDelegatedFromPath);

    /// <summary>Declares a role that an entity grants to its principal.</summary>
    /// <param name="securityRole">The granted role.</param>
    /// <param name="init">Optional settings of the role, such as a filter.</param>
    IVirtualPermissionRootSetup<TPermission> AddSecurityRole(SecurityRole securityRole, Action<IVirtualBindingInfoSettingsSetup<TPermission>>? init = null);
}