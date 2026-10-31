using System.Linq.Expressions;

using Anch.Core;
using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;

namespace Anch.SecuritySystem.VirtualPermission.DependencyInjection;

/// <summary>Registration of the VirtualPermission source.</summary>
public static class SecuritySystemSetupExtensions
{
    extension(ISecuritySystemSetup securitySystemSetup)
    {
        /// <summary>Adds a permission source that is computed from the application's domain entities, for example "a department head has the manager role on the department".</summary>
        /// <param name="principalAccessors">The principal of the entity.</param>
        /// <param name="setupAction">Declares the roles and the security contexts.</param>
        /// <typeparam name="TPrincipal">The principal entity type.</typeparam>
        /// <typeparam name="TPermission">The domain entity the permission is computed from.</typeparam>
        public ISecuritySystemSetup AddVirtualPermission<TPrincipal, TPermission>(
            PropertyAccessors<TPermission, TPrincipal> principalAccessors,
            Action<IVirtualPermissionRootSetup<TPermission>> setupAction)
            where TPrincipal : class
            where TPermission : class =>
            securitySystemSetup.Initialize<ISecuritySystemSetup, VirtualPermissionRootSetup<TPrincipal, TPermission>>(
                new VirtualPermissionRootSetup<TPrincipal, TPermission>(principalAccessors),
                setupAction);

        /// <summary>Adds a permission source that is computed from the application's domain entities, for example "a department head has the manager role on the department".</summary>
        /// <param name="principalPath">Selects the principal of the entity.</param>
        /// <param name="setupAction">Declares the roles and the security contexts.</param>
        /// <typeparam name="TPrincipal">The principal entity type.</typeparam>
        /// <typeparam name="TPermission">The domain entity the permission is computed from.</typeparam>
        public ISecuritySystemSetup AddVirtualPermission<TPrincipal, TPermission>(
            Expression<Func<TPermission, TPrincipal>> principalPath,
            Action<IVirtualPermissionRootSetup<TPermission>> setupAction)
            where TPrincipal : class
            where TPermission : class =>
            securitySystemSetup.AddVirtualPermission(
                principalPath.ToPropertyAccessors(), setupAction);
    }
}