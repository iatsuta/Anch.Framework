using System.Reflection;

namespace Anch.IdentitySource;

public interface IIdentityPropertyExtractor
{
    PropertyInfo? TryExtract(Type domainType);
}