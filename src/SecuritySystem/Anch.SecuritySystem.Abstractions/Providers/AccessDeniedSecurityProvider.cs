namespace Anch.SecuritySystem.Providers;

/// <summary>A provider that denies access to everything.</summary>
/// <typeparam name="TDomainObject">The type of the protected domain objects.</typeparam>
public class AccessDeniedSecurityProvider<TDomainObject>() : ConstSecurityProvider<TDomainObject>(false);