using System.Collections.Immutable;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public interface ISecurityContextInfoSource
{
    ImmutableArray<SecurityContextInfo> SecurityContextInfoList { get; }

    SecurityContextInfo GetSecurityContextInfo(Type type);

    SecurityContextInfo GetSecurityContextInfo(string name);

	SecurityContextInfo GetSecurityContextInfo(SecurityIdentity identity);
}
