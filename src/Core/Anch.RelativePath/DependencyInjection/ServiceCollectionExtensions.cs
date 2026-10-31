using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.RelativePath.DependencyInjection;

/// <summary>Registration of relative paths.</summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers a path to a single related object.</summary>
        /// <param name="path">Selects the related object.</param>
        /// <param name="key">Distinguishes several paths between the same types; <c>null</c> for the default one.</param>
        /// <typeparam name="TFrom">The source type.</typeparam>
        /// <typeparam name="TTo">The related type.</typeparam>
        public IServiceCollection AddRelativeDomainPath<TFrom, TTo>(Expression<Func<TFrom, TTo>> path, string? key = null)
        {
            var info = new SingleRelativeDomainPathInfo<TFrom, TTo>(path);

            if (key is null)
            {
                return services.AddSingleton<IRelativeDomainPathInfo<TFrom, TTo>>(info);
            }
            else
            {
                return services.AddKeyedSingleton<IRelativeDomainPathInfo<TFrom, TTo>>(key, info);
            }
        }

        /// <summary>Registers a path to a collection of related objects.</summary>
        /// <param name="path">Selects the related objects.</param>
        /// <param name="key">Distinguishes several paths between the same types; <c>null</c> for the default one.</param>
        /// <typeparam name="TFrom">The source type.</typeparam>
        /// <typeparam name="TTo">The related type.</typeparam>
        public IServiceCollection AddRelativeDomainPath<TFrom, TTo>(Expression<Func<TFrom, IEnumerable<TTo>>> path, string? key = null)
        {
            var info = new ManyRelativeDomainPathInfo<TFrom, TTo>(path);

            if (key is null)
            {
                return services.AddSingleton<IRelativeDomainPathInfo<TFrom, TTo>>(info);
            }
            else
            {
                return services.AddKeyedSingleton<IRelativeDomainPathInfo<TFrom, TTo>>(key, info);
            }
        }
    }
}