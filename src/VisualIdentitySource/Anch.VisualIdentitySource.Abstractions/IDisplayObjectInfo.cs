namespace Anch.VisualIdentitySource;

public interface IDisplayObjectInfo<in TDomainObject>
{
    Func<TDomainObject, string> DisplayFunc { get; }
}