using System.Linq.Expressions;

namespace Anch.VisualIdentitySource.DependencyInjection;

public interface IVisualIdentitySourceSetup
{
	IVisualIdentitySourceSetup SetSettings(VisualIdentityPropertySourceSettings settings);

	IVisualIdentitySourceSetup SetName<TDomainObject>(Expression<Func<TDomainObject, string>> namePath);

	IVisualIdentitySourceSetup SetDisplay<TDomainObject>(Func<TDomainObject, string> displayFunc)
		where TDomainObject : class;
}