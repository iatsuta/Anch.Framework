using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.DependencyInjection;

public interface IServiceInitializer<in TServiceContainer>
{
    void Initialize(TServiceContainer services);
}

public interface IServiceInitializer : IServiceInitializer<IServiceCollection>;