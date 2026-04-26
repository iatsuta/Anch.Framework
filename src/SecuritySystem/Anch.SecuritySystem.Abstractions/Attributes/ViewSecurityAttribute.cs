using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class ViewSecurityAttribute() : FromKeyedServicesAttribute(nameof(SecurityRule.View));
