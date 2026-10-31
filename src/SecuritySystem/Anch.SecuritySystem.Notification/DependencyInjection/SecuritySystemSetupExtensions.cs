using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;

namespace Anch.SecuritySystem.Notification.DependencyInjection;

/// <summary>Registration of notifications.</summary>
public static class SecuritySystemSetupExtensions
{
    extension(ISecuritySystemSetup securitySystemSetup)
    {
        /// <summary>Adds the services that determine who must be notified, based on roles and security contexts.</summary>
        /// <param name="setupAction">Optional configuration.</param>
        public ISecuritySystemSetup AddNotification(Action<ISecuritySystemNotificationSetup>? setupAction = null) =>
            securitySystemSetup.Initialize<ISecuritySystemSetup, SecuritySystemNotificationSetup>(new(), setupAction);
    }
}