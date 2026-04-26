// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public interface IContextSecurityPath
{
    Type SecurityContextType { get; }

    bool Required { get; }

    string? Key { get; }
}
