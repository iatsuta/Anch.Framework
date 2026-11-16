namespace Anch.SecuritySystem.Providers;

/// <summary>A provider that grants access to everything; injected with <c>[DisabledSecurity]</c>.</summary>
/// <typeparam name="TDomainObject">The type of the protected domain objects.</typeparam>
public class DisabledSecurityProvider<TDomainObject>() : ConstSecurityProvider<TDomainObject>(true);