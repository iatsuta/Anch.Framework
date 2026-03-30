using System.Reflection;

namespace CommonFramework.IdentitySource;

public class IdentityPropertyExtractor(IdentityPropertySourceSettings settings) : IIdentityPropertyExtractor
{
	public PropertyInfo? TryExtract(Type domainType)
	{
		return domainType.GetProperty(settings.PropertyName, BindingFlags.Public | BindingFlags.Instance);
	}
}