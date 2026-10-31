namespace Anch.VisualIdentitySource;

/// <summary>How a domain object is shown to people as a single string.</summary>
/// <param name="DisplayFunc">Converts an object to its display text.</param>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public record DisplayObjectInfo<TDomainObject>(Func<TDomainObject, string> DisplayFunc) : DisplayObjectInfo, IDisplayObjectInfo<TDomainObject>
    where TDomainObject : class
{
    public override Type DomainObjectType { get; } = typeof(TDomainObject);

    /// <summary>Display by <c>ToString()</c>, falling back to the type name when it returns <c>null</c>.</summary>
    public static DisplayObjectInfo<TDomainObject> Default { get; } = new(v => v.ToString() ?? typeof(TDomainObject).Name);
}

/// <summary>Untyped description of how a domain type is displayed.</summary>
public abstract record DisplayObjectInfo
{
    public abstract Type DomainObjectType { get; }
}