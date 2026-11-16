namespace Anch.VisualIdentitySource;

/// <summary>Provides the display description of domain types: the configured display function or one based on the name property.</summary>
public interface IDisplayObjectInfoSource
{
    /// <summary>Returns the display description of a type.</summary>
    /// <typeparam name="TDomainObject">The domain type.</typeparam>
    DisplayObjectInfo<TDomainObject> GetDisplayObjectInfo<TDomainObject>()
        where TDomainObject : class;
}