using System.Reflection;

namespace CommonFramework.IdentitySource;

public interface IIdentityPropertyExtractor
{
    PropertyInfo? TryExtract(Type domainType);
}