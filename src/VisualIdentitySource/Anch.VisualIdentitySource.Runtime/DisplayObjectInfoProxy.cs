namespace Anch.VisualIdentitySource;

/// <summary>Injectable display description of a type; resolved through the display info source.</summary>
/// <param name="displayObjectInfoSource">Finds the description.</param>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public class DisplayObjectInfoProxy<TDomainObject>(IDisplayObjectInfoSource displayObjectInfoSource) : IDisplayObjectInfo<TDomainObject>
    where TDomainObject : class
{
    /// <inheritdoc/>
    public Func<TDomainObject, string> DisplayFunc { get; } = displayObjectInfoSource.GetDisplayObjectInfo<TDomainObject>().DisplayFunc;
}