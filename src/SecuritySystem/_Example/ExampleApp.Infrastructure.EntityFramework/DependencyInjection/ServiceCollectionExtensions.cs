using ExampleApp.Infrastructure.Services;

using GenericQueryable.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddEntityFrameworkInfrastructure(IConfiguration configuration)
        {
            return services
                .AddDbContext<AppDbContext>(optionsBuilder => optionsBuilder
                    .UseSqlite(configuration.GetConnectionString("DefaultConnection"))
                    .UseLazyLoadingProxies()
                    .UseGenericQueryable())
                .AddScoped(typeof(IDal<>), typeof(EfDal<>))
                .AddScoped<AutoCommitSession>()
                .AddScoped<IDbSchemaInitializer, DbSchemaInitializer>();
        }
    }
}