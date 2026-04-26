using Microsoft.Extensions.Configuration;

namespace ExampleApp.Infrastructure.Services;

public static class ConfigurationExtensions
{
    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{name}' is not configured.");
        }

        return connectionString;
    }
}