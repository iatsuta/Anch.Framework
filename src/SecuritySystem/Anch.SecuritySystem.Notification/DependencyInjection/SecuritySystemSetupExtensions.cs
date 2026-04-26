using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;

namespace Anch.SecuritySystem.Notification.DependencyInjection;

public static class SecuritySystemSetupExtensions
{
    extension(ISecuritySystemSetup securitySystemSetup)
    {
        public ISecuritySystemSetup AddNotification(Action<ISecuritySystemNotificationSetup>? setupAction = null) =>
            securitySystemSetup.Initialize<ISecuritySystemSetup, SecuritySystemNotificationSetup>(new(), setupAction);
    }
}