using System.Linq.Expressions;

namespace Anch.IdentitySource.DependencyInjection;

/// <summary>Fluent configuration of the IdentitySource module.</summary>
public interface IIdentitySourceSetup
{
    /// <summary>Changes the convention used to locate identifier properties (for example the property name).</summary>
    /// <param name="settings">The convention settings.</param>
    IIdentitySourceSetup SetSettings(IdentityPropertySourceSettings settings);

    /// <summary>Declares the identifier property of a type explicitly, overriding the convention.</summary>
    /// <param name="idPath">Selects the identifier property.</param>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    /// <typeparam name="TIdent">The type of the identifier.</typeparam>
    IIdentitySourceSetup SetId<TDomainObject, TIdent>(Expression<Func<TDomainObject, TIdent>> idPath)
        where TIdent : notnull;
}