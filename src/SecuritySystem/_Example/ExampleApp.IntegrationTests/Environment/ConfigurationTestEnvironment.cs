using Anch.Testing.Database.ConnectionStringManagement;
using Anch.Testing.Database.Initializers;

using ExampleApp.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.IntegrationTests.Environment;

public abstract class ConfigurationTestEnvironment : DatabaseTestEnvironment
{
    private string DefaultConnectionStringName => field ??= this.GetDefaultConnectionStringName();

    private IConfiguration MainConfiguration => field ??= this.GetMainConfiguration();

    protected override TestConnectionString MainConnectionString => field ??=

        field ??= new(this.MainConfiguration.GetRequiredConnectionString(this.DefaultConnectionStringName));

    protected override IServiceProvider BuildServiceProvider(IServiceCollection services, TestConnectionString actualConnectionString) =>
        this.BuildServiceProvider(services, this.GetActualConfiguration(actualConnectionString));

    protected abstract IConfiguration GetMainConfiguration();

    protected abstract IServiceProvider BuildServiceProvider(IServiceCollection services, IConfiguration configuration);

    protected virtual string GetDefaultConnectionStringName() => "DefaultConnection";

    private IConfiguration GetActualConfiguration(TestConnectionString actualConnectionString)
    {
        if (actualConnectionString == this.MainConnectionString)
        {
            return this.MainConfiguration;
        }
        else
        {
            return new ConfigurationBuilder()
                .AddConfiguration(this.MainConfiguration)
                .AddInMemoryCollection(new Dictionary<string, string?>
                    { [$"ConnectionStrings:{this.DefaultConnectionStringName}"] = actualConnectionString.Value })
                .Build();
        }
    }
}