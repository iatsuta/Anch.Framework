using System.Reflection;

namespace Anch.VisualIdentitySource;

public interface IVisualIdentityPropertyExtractor
{
	PropertyInfo? TryExtract(Type domainType);
}