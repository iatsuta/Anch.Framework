// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>A security path node that points to a security context type.</summary>
public interface IContextSecurityPath
{
    Type SecurityContextType { get; }

    bool Required { get; }

    string? Key { get; }
}
