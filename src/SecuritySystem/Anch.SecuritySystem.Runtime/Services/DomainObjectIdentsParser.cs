using System.Collections.Concurrent;
using Anch.IdentitySource;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Services;

public class DomainObjectIdentsParser(IServiceProvider serviceProvider, IIdentityInfoSource identityInfoSource) : IDomainObjectIdentsParser
{
	private readonly ConcurrentDictionary<Type, IIdentsParser> parsersCache = new();

	public Array Parse(Type domainObjectType, IEnumerable<string> idents) =>
        this.parsersCache.GetOrAdd(domainObjectType, _ =>
			{
				var identityInfo = identityInfoSource.GetIdentityInfo(domainObjectType);

				return (IIdentsParser)serviceProvider.GetRequiredService(typeof(IIdentsParser<>).MakeGenericType(identityInfo.IdentityType));
			})
			.Parse(idents);

}