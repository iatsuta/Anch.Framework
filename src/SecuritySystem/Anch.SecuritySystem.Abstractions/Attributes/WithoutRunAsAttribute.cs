using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Attributes;

public class WithoutRunAsAttribute() : FromKeyedServicesAttribute(nameof(SecurityRuleCredential.CurrentUserWithoutRunAsCredential));
