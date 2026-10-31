using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Attributes;

/// <summary>Injects the provider that grants access to everything (<see cref="SecurityRule.Disabled"/>) into a constructor parameter.</summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class DisabledSecurityAttribute() : FromKeyedServicesAttribute(nameof(SecurityRule.Disabled));
