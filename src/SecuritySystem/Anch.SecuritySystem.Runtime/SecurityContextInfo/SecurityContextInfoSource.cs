using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using Anch.Core;
using Anch.SecuritySystem.Services;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public class SecurityContextInfoSource : ISecurityContextInfoSource
{
    private readonly FrozenDictionary<Type, SecurityContextInfo> typeDict;

    private readonly FrozenDictionary<TypedSecurityIdentity, SecurityContextInfo> identityDict;

    private readonly FrozenDictionary<string, SecurityContextInfo> nameDict;


    private readonly ConcurrentDictionary<SecurityIdentity, SecurityContextInfo> baseIdentityCache = new();

    private readonly ISecurityIdentityConverter rootIdentityConverter;

    public SecurityContextInfoSource(IServiceProxyFactory serviceProxyFactory, IEnumerable<SecurityContextInfo> securityContextInfoList)
    {
        this.SecurityContextInfoList = [..securityContextInfoList];

        this.typeDict = this.SecurityContextInfoList.ToFrozenDictionary(v => v.Type);
        this.identityDict = this.typeDict.Values.ToFrozenDictionary(v => v.Identity);
        this.nameDict = this.typeDict.Values.ToFrozenDictionary(v => v.Name);

        this.rootIdentityConverter =
            serviceProxyFactory.Create<ISecurityIdentityConverter, RootSecurityIdentityConverter>(
                this.SecurityContextInfoList.Select(sr => sr.Identity.IdentType).Distinct());
    }

    public ImmutableArray<SecurityContextInfo> SecurityContextInfoList { get; }

    public virtual SecurityContextInfo GetSecurityContextInfo(Type type) =>
        this.typeDict[type];

    public SecurityContextInfo GetSecurityContextInfo(string name) =>
        this.nameDict[name];

    public SecurityContextInfo GetSecurityContextInfo(SecurityIdentity identity)
    {
        return this.baseIdentityCache.GetOrAdd(identity, _ =>
        {
            if (this.rootIdentityConverter.TryConvert(identity) is { } convertedIdentity &&
                this.identityDict.TryGetValue(convertedIdentity, out var securityContextInfo))
            {
                return securityContextInfo;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(identity), $"{nameof(SecurityContextInfo)} with {nameof(identity)} '{identity}' not found");
            }
        });
    }
}