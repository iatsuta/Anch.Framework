using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class DisabledSecurityAttribute() : FromKeyedServicesAttribute(nameof(SecurityRule.Disabled));
