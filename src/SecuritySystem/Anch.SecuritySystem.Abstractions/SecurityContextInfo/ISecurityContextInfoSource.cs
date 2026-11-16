using System.Collections.Immutable;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>The registry of configured security context types.</summary>
public interface ISecurityContextInfoSource
{
    /// <summary>All registered security context types.</summary>
    ImmutableArray<SecurityContextInfo> SecurityContextInfoList { get; }

    /// <summary>Returns the registration of a context type.</summary>
    /// <param name="type">The CLR type.</param>
    SecurityContextInfo GetSecurityContextInfo(Type type);

    /// <summary>Returns the registration of a context type by name.</summary>
    /// <param name="name">The context type name.</param>
    SecurityContextInfo GetSecurityContextInfo(string name);

    /// <summary>Returns the registration of a context type by identifier.</summary>
    /// <param name="identity">The context type identifier.</param>
    SecurityContextInfo GetSecurityContextInfo(SecurityIdentity identity);
}
