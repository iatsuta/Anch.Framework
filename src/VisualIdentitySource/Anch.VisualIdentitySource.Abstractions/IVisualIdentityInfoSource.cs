namespace Anch.VisualIdentitySource;

/// <summary>Finds the name property of an arbitrary domain type. The property is configured explicitly or located by convention (by default the first of <c>Login</c>, <c>Name</c>, <c>Code</c>).</summary>
public interface IVisualIdentityInfoSource
{
    /// <summary>Returns the name description of a type; throws if none can be determined.</summary>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    VisualIdentityInfo<TDomainObject> GetVisualIdentityInfo<TDomainObject>();

    /// <summary>Returns the name description of a type, or <c>null</c> if none can be determined.</summary>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    VisualIdentityInfo<TDomainObject>? TryGetVisualIdentityInfo<TDomainObject>();

    /// <summary>Returns the name description of a type; throws if none can be determined.</summary>
    /// <param name="domainObjectType">The domain type.</param>
    VisualIdentityInfo GetVisualIdentityInfo(Type domainObjectType);

    /// <summary>Returns the name description of a type, or <c>null</c> if none can be determined.</summary>
    /// <param name="domainObjectType">The domain type.</param>
    VisualIdentityInfo? TryGetVisualIdentityInfo(Type domainObjectType);
}