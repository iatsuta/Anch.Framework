using Anch.Testing.Database.ConnectionStringManagement;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Configuration;

public abstract class ConfigurationTestEnvironment : DatabaseTestEnvironment
{
    private string MainConnectionStringName => field ??= this.GetMainConnectionStringName();

    private IConfiguration MainConfiguration => field ??= this.GetMainConfiguration();

    protected override TestConnectionString MainConnectionString =>
        field ??= new TestConnectionString(this.MainConfiguration.GetRequiredConnectionString(this.MainConnectionStringName));

    protected override IServiceProvider BuildServiceProvider(IServiceCollection services, TestConnectionString actualConnectionString)
    {
        var actualConfiguration = this.GetActualConfiguration(actualConnectionString);

        return this.BuildServiceProvider(services.AddSingleton(actualConfiguration), actualConfiguration);
    }

    protected abstract IConfiguration GetMainConfiguration();

    protected abstract IServiceProvider BuildServiceProvider(IServiceCollection services, IConfiguration configuration);

    protected virtual string GetMainConnectionStringName() => "DefaultConnection";

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
                    { [$"ConnectionStrings:{this.MainConnectionStringName}"] = actualConnectionString.Value })
                .Build();
        }
    }
}