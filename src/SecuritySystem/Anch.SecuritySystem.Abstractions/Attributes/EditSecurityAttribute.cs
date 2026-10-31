using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Attributes;

/// <summary>Injects the <see cref="SecurityRule.Edit"/> provider of the parameter's domain type into a constructor parameter.</summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class EditSecurityAttribute() : FromKeyedServicesAttribute(nameof(SecurityRule.Edit));
