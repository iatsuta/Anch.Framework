using Anch.GenericQueryable.DependencyInjection;

namespace Anch.GenericQueryable.EntityFramework;

public interface IEfGenericQueryableExtensionInnerSetup
{
    void Initialize(IGenericQueryableSetup setup);
}