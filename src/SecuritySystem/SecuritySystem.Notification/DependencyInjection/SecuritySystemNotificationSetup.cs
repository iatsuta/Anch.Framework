using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.DependencyInjection;

namespace SecuritySystem.Notification.DependencyInjection;

public class SecuritySystemNotificationSetup : ISecuritySystemNotificationSetup, IServiceInitializer<ISecuritySystemSetup>
{
    public void Initialize(ISecuritySystemSetup securitySystemSetup) => this.RegisterGeneralServices(securitySystemSetup);

    private ISecuritySystemSetup RegisterGeneralServices(ISecuritySystemSetup settings) =>

        settings
            .AddExtensions(services =>
            {
                services
                    .AddScoped(typeof(IDirectLevelExtractor<>), typeof(DirectLevelExtractor<>))
                    .AddScoped(typeof(INotificationPermissionExtractor<>), typeof(NotificationPermissionExtractor<>))
                    .AddScoped(typeof(INotificationPrincipalExtractor<>), typeof(NotificationPrincipalExtractor<>))

                    .AddScoped(typeof(INotificationGeneralPermissionFilterFactory<>), typeof(NotificationGeneralPermissionFilterFactory<>));
            });
}