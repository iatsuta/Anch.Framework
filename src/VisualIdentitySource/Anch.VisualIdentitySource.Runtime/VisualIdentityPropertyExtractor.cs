using System.Reflection;

namespace Anch.VisualIdentitySource;

public class VisualIdentityPropertyExtractor(VisualIdentityPropertySourceSettings settings) : IVisualIdentityPropertyExtractor
{
	public PropertyInfo? TryExtract(Type domainType)
	{
		var request =

			from propertyName in settings.PropertyNameList

			let property = domainType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)

			where property != null && property.PropertyType == typeof(string)

			select property;

		return request.FirstOrDefault();
	}
}