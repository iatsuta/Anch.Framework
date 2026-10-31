using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Attributes;

/// <summary>Injects the <see cref="SecurityRule.View"/> provider of the parameter's domain type into a constructor parameter.</summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class ViewSecurityAttribute() : FromKeyedServicesAttribute(nameof(SecurityRule.View));
