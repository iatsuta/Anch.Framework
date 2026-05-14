using Anch.Testing.Database.ConnectionStringManagement;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Configuration;

public abstract class ConfigurationTestEnvironment : DatabaseTestEnvironment
{
    protected abstract IConfiguration RawConfiguration { get; }

    protected abstract string ConnectionStringName { get; }

    protected override TestConnectionString RawConnectionString =>

        field ??= new TestConnectionString(this.RawConfiguration.GetRequiredConnectionString(this.ConnectionStringName));

    protected override IServiceProvider BuildServiceProvider(IServiceCollection services, TestConnectionString actualConnectionString)
    {
        var actualConfiguration = this.GetActualConfiguration(actualConnectionString);

        return this.BuildServiceProvider(services.AddSingleton(actualConfiguration), actualConfiguration);
    }

    protected abstract IServiceProvider BuildServiceProvider(IServiceCollection services, IConfiguration configuration);

    private IConfiguration GetActualConfiguration(TestConnectionString actualConnectionString)
    {
        if (actualConnectionString == this.RawConnectionString)
        {
            return this.RawConfiguration;
        }
        else
        {
            return new ConfigurationBuilder()
                .AddConfiguration(this.RawConfiguration)
                .AddInMemoryCollection(new Dictionary<string, string?>
                    { [$"ConnectionStrings:{this.ConnectionStringName}"] = actualConnectionString.Value })
                .Build();
        }
    }
}