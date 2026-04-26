using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class EditSecurityAttribute() : FromKeyedServicesAttribute(nameof(SecurityRule.Edit));
