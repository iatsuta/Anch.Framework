using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.RelativePath.DependencyInjection;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddRelativeDomainPath<TFrom, TTo>(Expression<Func<TFrom, TTo>> path, string? key = null)
		{
			var info = new SingleRelativeDomainPathInfo<TFrom, TTo>(path);

			if (key == null)
			{
				return services.AddSingleton<IRelativeDomainPathInfo<TFrom, TTo>>(info);
			}
			else
			{
				return services.AddKeyedSingleton<IRelativeDomainPathInfo<TFrom, TTo>>(key, info);
			}
		}

		public IServiceCollection AddRelativeDomainPath<TFrom, TTo>(Expression<Func<TFrom, IEnumerable<TTo>>> path, string? key = null)
		{
			var info = new ManyRelativeDomainPathInfo<TFrom, TTo>(path);

			if (key == null)
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