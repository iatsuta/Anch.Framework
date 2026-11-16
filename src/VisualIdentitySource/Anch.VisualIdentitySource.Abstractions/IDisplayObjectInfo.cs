namespace Anch.VisualIdentitySource;

/// <summary>Describes how a domain object is shown to people.</summary>
/// <typeparam name="TDomainObject">The domain type.</typeparam>
public interface IDisplayObjectInfo<in TDomainObject>
{
    /// <summary>Converts an object to its display text.</summary>
    Func<TDomainObject, string> DisplayFunc { get; }
}