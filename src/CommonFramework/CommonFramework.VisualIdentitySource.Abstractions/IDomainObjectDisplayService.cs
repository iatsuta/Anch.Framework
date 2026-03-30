namespace CommonFramework.VisualIdentitySource;

public interface IDomainObjectDisplayService
{
	string Format<TDomainObject>(TDomainObject domainObject)
		where TDomainObject : class;
}