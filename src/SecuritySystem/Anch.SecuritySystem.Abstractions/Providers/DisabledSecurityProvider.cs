namespace Anch.SecuritySystem.Providers;

public class DisabledSecurityProvider<TDomainObject>() : ConstSecurityProvider<TDomainObject>(true);