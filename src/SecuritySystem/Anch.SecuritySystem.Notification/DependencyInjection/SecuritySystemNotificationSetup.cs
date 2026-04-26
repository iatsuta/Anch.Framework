using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Notification.DependencyInjection;

public class SecuritySystemNotificationSetup : ISecuritySystemNotificationSetup, IServiceInitializer<ISecuritySystemSetup>
{
    public void Initialize(ISecuritySystemSetup securitySystemSetup) => this.RegisterGeneralServices(securitySystemSetup);

    private ISecuritySystemSetup RegisterGeneralServices(ISecuritySystemSetup settings) =>

        settings
            .AddExtensions(services =>
            {
                services
                    .AddSingleton<INotificationFilterGroupConverter, NotificationFilterGroupConverter>()
                    .AddScoped(typeof(IDirectLevelExtractor<>), typeof(DirectLevelExtractor<>))
                    .AddScoped(typeof(INotificationPermissionExtractor<>), typeof(NotificationPermissionExtractor<>))
                    .AddScoped(typeof(INotificationPrincipalExtractor<>), typeof(NotificationPrincipalExtractor<>))

                    .AddScoped(typeof(INotificationPermissionFilterFactory<>), typeof(NotificationGeneralPermissionFilterFactory<>));
            });
}