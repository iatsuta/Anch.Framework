using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.HierarchicalExpand.DependencyInjection;

/// <summary>Registration of the HierarchicalExpand module.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds hierarchy expansion services. Also registers identity source and service proxy factory.
        /// The container must provide <c>IQueryableSource</c> and <c>IGenericRepository</c>.
        /// </summary>
        /// <param name="setupAction">Registers the hierarchical types.</param>
        public IServiceCollection AddHierarchicalExpand(Action<IHierarchicalExpandSetup>? setupAction = null) =>
            services.Initialize<HierarchicalExpandSetup>(setupAction);
    }
}