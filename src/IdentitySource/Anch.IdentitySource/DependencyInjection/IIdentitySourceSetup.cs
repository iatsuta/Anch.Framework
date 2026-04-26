using System.Linq.Expressions;

namespace Anch.IdentitySource.DependencyInjection;

public interface IIdentitySourceSetup
{
	IIdentitySourceSetup SetSettings(IdentityPropertySourceSettings settings);

	IIdentitySourceSetup SetId<TDomainObject, TIdent>(Expression<Func<TDomainObject, TIdent>> idPath)
		where TIdent : notnull;
}