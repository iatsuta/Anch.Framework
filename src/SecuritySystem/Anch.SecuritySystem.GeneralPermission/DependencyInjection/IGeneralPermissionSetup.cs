using System.Linq.Expressions;

using Anch.Core;
using Anch.SecuritySystem.GeneralPermission.Validation;

namespace Anch.SecuritySystem.GeneralPermission.DependencyInjection;

/// <summary>Optional settings of the GeneralPermission source: how the application's permission entity stores the period, comment and delegation.</summary>
/// <typeparam name="TPrincipal">The principal entity type.</typeparam>
/// <typeparam name="TPermission">The permission entity type.</typeparam>
/// <typeparam name="TSecurityRole">The role entity type.</typeparam>
/// <typeparam name="TPermissionRestriction">The permission restriction entity type.</typeparam>
public interface IGeneralPermissionSetup<out TPrincipal, TPermission, TSecurityRole, TPermissionRestriction>
{
    /// <summary>Sets the validity period properties of the permission.</summary>
    /// <param name="startDatePropertyAccessor">Accessors of the start date; <c>null</c> if there is none.</param>
    /// <param name="endDatePropertyAccessor">Accessors of the end date; <c>null</c> if there is none.</param>
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionPeriod(
        PropertyAccessors<TPermission, DateTime?>? startDatePropertyAccessor,
        PropertyAccessors<TPermission, DateTime?>? endDatePropertyAccessor);

    /// <summary>Sets the validity period properties of the permission.</summary>
    /// <param name="startDatePath">Selects the start date; <c>null</c> if there is none.</param>
    /// <param name="endDatePath">Selects the end date; <c>null</c> if there is none.</param>
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionPeriod(
        Expression<Func<TPermission, DateTime?>>? startDatePath,
        Expression<Func<TPermission, DateTime?>>? endDatePath);

    /// <summary>Sets the comment property of the permission.</summary>
    /// <param name="commentPath">Selects the comment.</param>
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionComment(
        Expression<Func<TPermission, string>> commentPath);

    /// <summary>Sets the property that references the permission this one was delegated from.</summary>
    /// <param name="delegatedFromPath">Selects the source permission.</param>
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionDelegation(
        Expression<Func<TPermission, TPermission?>> delegatedFromPath);

    /// <summary>Sets the description property of the role entity.</summary>
    /// <param name="descriptionPath">Selects the description; <c>null</c> if there is none.</param>
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetSecurityRoleDescription(
        Expression<Func<TSecurityRole, string>>? descriptionPath);

    /// <summary>Forbids changing principals and permissions through the security system.</summary>
    /// <param name="value"><c>true</c> to make the source read-only.</param>
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetReadonly(bool value = true);

    /// <summary>Replaces the rule that decides whether two permissions are the same when permissions are updated.</summary>
    /// <typeparam name="TComparer">The comparer type.</typeparam>
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetCustomPermissionEqualityComparer<TComparer>()
        where TComparer : IPermissionEqualityComparer<TPermission, TPermissionRestriction>;

    /// <summary>Replaces the service that creates and changes permissions.</summary>
    /// <typeparam name="TPermissionManagementService">The service type.</typeparam>
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetCustomPermissionManagementService<TPermissionManagementService>()
        where TPermissionManagementService : IPermissionManagementService<TPrincipal, TPermission, TPermissionRestriction>;
}