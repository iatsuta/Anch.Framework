using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddHierarchicalExpand(Action<IHierarchicalExpandSetup>? setupAction = null) =>
            services.Initialize<HierarchicalExpandSetup>(setupAction);
    }
}