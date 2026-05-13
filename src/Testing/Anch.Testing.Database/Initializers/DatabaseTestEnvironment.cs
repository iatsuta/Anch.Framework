using Anch.Testing.Database.ConnectionStringManagement;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Testing.Database.Initializers;

public abstract class DatabaseTestEnvironment : ITestEnvironment
{
    private IServiceProvider? mainServiceProvider;

    protected abstract TestConnectionString MainConnectionString { get; }

    public IServiceProvider BuildServiceProvider(IServiceCollection services, ServiceProviderIndex serviceProviderIndex)
    {
        if (serviceProviderIndex.IsMain)
        {
            return this.mainServiceProvider ??= this.BuildServiceProvider(services, this.MainConnectionString);
        }
        else
        {
            var actualConnectionString = (this.mainServiceProvider ?? throw new InvalidOperationException("Main service provider is not initialized."))
                .GetRequiredService<IActualConnectionStringResolver>().GetActualConnectionString(serviceProviderIndex);

            return this.BuildServiceProvider(services, actualConnectionString);
        }
    }

    protected abstract IServiceProvider BuildServiceProvider(IServiceCollection services, TestConnectionString actualConnectionString);
}