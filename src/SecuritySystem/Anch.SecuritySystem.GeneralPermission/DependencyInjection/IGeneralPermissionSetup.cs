using System.Linq.Expressions;

using Anch.Core;
using Anch.SecuritySystem.GeneralPermission.Validation;

namespace Anch.SecuritySystem.GeneralPermission.DependencyInjection;

public interface IGeneralPermissionSetup<out TPrincipal, TPermission, TSecurityRole, TPermissionRestriction>
{
    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionPeriod(
        PropertyAccessors<TPermission, DateTime?>? startDatePropertyAccessor,
        PropertyAccessors<TPermission, DateTime?>? endDatePropertyAccessor);

    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionPeriod(
        Expression<Func<TPermission, DateTime?>>? startDatePath,
        Expression<Func<TPermission, DateTime?>>? endDatePath);

    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionComment(
        Expression<Func<TPermission, string>> commentPath);

    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetPermissionDelegation(
        Expression<Func<TPermission, TPermission?>> delegatedFromPath);

    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetSecurityRoleDescription(
        Expression<Func<TSecurityRole, string>>? descriptionPath);

    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetReadonly(bool value = true);

    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetCustomPermissionEqualityComparer<TComparer>()
        where TComparer : IPermissionEqualityComparer<TPermission, TPermissionRestriction>;

    IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction> SetCustomPermissionManagementService<TPermissionManagementService>()
        where TPermissionManagementService : IPermissionManagementService<TPrincipal, TPermission, TPermissionRestriction>;
}