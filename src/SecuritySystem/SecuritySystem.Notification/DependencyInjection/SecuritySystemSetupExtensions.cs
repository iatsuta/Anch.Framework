using CommonFramework.DependencyInjection;

using SecuritySystem.DependencyInjection;

namespace SecuritySystem.Notification.DependencyInjection;

public static class SecuritySystemSetupExtensions
{
    extension(ISecuritySystemSetup securitySystemSetup)
    {
        public ISecuritySystemSetup AddNotification(Action<ISecuritySystemNotificationSetup>? setupAction = null) =>
            securitySystemSetup.Initialize<ISecuritySystemSetup, SecuritySystemNotificationSetup>(new(), setupAction);
    }
}