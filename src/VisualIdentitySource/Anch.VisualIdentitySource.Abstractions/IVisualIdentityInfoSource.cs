namespace Anch.VisualIdentitySource;

public interface IVisualIdentityInfoSource
{
	VisualIdentityInfo<TDomainObject> GetVisualIdentityInfo<TDomainObject>();

	VisualIdentityInfo<TDomainObject>? TryGetVisualIdentityInfo<TDomainObject>();

    VisualIdentityInfo GetVisualIdentityInfo(Type domainObjectType);

    VisualIdentityInfo? TryGetVisualIdentityInfo(Type domainObjectType);
}