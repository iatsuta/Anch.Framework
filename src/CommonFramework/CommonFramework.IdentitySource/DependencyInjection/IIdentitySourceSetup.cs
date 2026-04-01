using System.Linq.Expressions;

namespace CommonFramework.IdentitySource.DependencyInjection;

public interface IIdentitySourceSetup
{
	IIdentitySourceSetup SetSettings(IdentityPropertySourceSettings settings);

	IIdentitySourceSetup SetId<TDomainObject, TIdent>(Expression<Func<TDomainObject, TIdent>> idPath)
		where TIdent : notnull;
}