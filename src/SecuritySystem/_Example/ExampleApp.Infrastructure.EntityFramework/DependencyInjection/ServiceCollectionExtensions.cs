using ExampleApp.Infrastructure.Services;

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
                .AddDbContext<AppDbContext>()
                .AddScoped(typeof(IDal<>), typeof(EfDal<>))
                .AddScoped<EfAutoCommitSession>()
                .AddSingleton<IEmptySchemaInitializer, EfEmptySchemaInitializer>();
        }
    }
}