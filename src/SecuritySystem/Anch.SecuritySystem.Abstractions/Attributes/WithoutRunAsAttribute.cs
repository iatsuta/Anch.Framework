using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Attributes;

/// <summary>Injects the service variant that ignores an active RunAs session.</summary>
public class WithoutRunAsAttribute() : FromKeyedServicesAttribute(nameof(SecurityRuleCredential.CurrentUserWithoutRunAsCredential));
