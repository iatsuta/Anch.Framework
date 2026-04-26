using Microsoft.Extensions.DependencyInjection;

namespace Anch.DependencyInjection;

public interface IServiceInitializer<in TServiceContainer>
{
    void Initialize(TServiceContainer services);
}

public interface IServiceInitializer : IServiceInitializer<IServiceCollection>;