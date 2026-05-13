using Microsoft.Extensions.Configuration;

namespace ExampleApp.Infrastructure.Services;

public class MainConnectionStringSource(IConfiguration configuration) : IMainConnectionStringSource
{
    public const string DefaultName = "DefaultConnection";

    public string ConnectionString { get; } = configuration.GetConnectionString(DefaultName) ??
                                              throw new InvalidOperationException($"Connection string '{DefaultName}' not found.");
}