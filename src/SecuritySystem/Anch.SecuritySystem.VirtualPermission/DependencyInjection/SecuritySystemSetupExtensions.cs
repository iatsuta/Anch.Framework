using System.Linq.Expressions;
using Anch.Core;
using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;

namespace Anch.SecuritySystem.VirtualPermission.DependencyInjection;

public static class SecuritySystemSetupExtensions
{
    extension(ISecuritySystemSetup securitySystemSetup)
    {
        public ISecuritySystemSetup AddVirtualPermission<TPrincipal, TPermission>(
            PropertyAccessors<TPermission, TPrincipal> principalAccessors,
            Action<IVirtualPermissionRootSetup<TPermission>> setupAction)
            where TPrincipal : class
            where TPermission : class =>
            securitySystemSetup.Initialize<ISecuritySystemSetup, VirtualPermissionRootSetup<TPrincipal, TPermission>>(
                new VirtualPermissionRootSetup<TPrincipal, TPermission>(principalAccessors),
                setupAction);

        public ISecuritySystemSetup AddVirtualPermission<TPrincipal, TPermission>(
            Expression<Func<TPermission, TPrincipal>> principalPath,
            Action<IVirtualPermissionRootSetup<TPermission>> setupAction)
            where TPrincipal : class
            where TPermission : class =>
            securitySystemSetup.AddVirtualPermission(
                principalPath.ToPropertyAccessors(), setupAction);
    }
}