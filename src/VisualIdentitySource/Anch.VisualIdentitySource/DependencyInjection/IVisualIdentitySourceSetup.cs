using System.Linq.Expressions;

namespace Anch.VisualIdentitySource.DependencyInjection;

/// <summary>Fluent configuration of the VisualIdentitySource module.</summary>
public interface IVisualIdentitySourceSetup
{
    /// <summary>Changes the convention used to locate name properties (the candidate property names).</summary>
    /// <param name="settings">The convention settings.</param>
    IVisualIdentitySourceSetup SetSettings(VisualIdentityPropertySourceSettings settings);

    /// <summary>Declares the name property of a type explicitly, overriding the convention.</summary>
    /// <param name="namePath">Selects the name property.</param>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    IVisualIdentitySourceSetup SetName<TDomainObject>(Expression<Func<TDomainObject, string>> namePath);

    /// <summary>Declares how a type is converted to display text, overriding the name property.</summary>
    /// <param name="displayFunc">Converts an object to its display text.</param>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    IVisualIdentitySourceSetup SetDisplay<TDomainObject>(Func<TDomainObject, string> displayFunc)
        where TDomainObject : class;
}