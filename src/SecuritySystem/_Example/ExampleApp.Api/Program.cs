using Anch.DependencyInjection;
using Anch.SecuritySystem.Configurator;
using ExampleApp.Infrastructure.DependencyInjection;

using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExampleApp.Api;

public static class GenericProgram
{
    public static async Task Main(string[] args, Action<WebApplicationBuilder> init)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.Sources.Clear();
        builder.Configuration.AddJsonFile("appSettings.json", false, true);

        builder.Host
            .UseDefaultServiceProvider(x =>
            {
                x.ValidateScopes = true;
                x.ValidateOnBuild = true;
            });

        builder
            .Services
            .AddInfrastructure(builder.Configuration)
            .AddConfigurator()

            .AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate();

        init(builder);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthorization(options =>
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        builder.Services.AddControllers(x => x.EnableEndpointRouting = false);

        builder.Services
            .AddValidator(new DuplicateServiceUsageValidator([typeof(ILoggerFactory), typeof(IMemoryPoolFactory<byte>)]))
            .Validate();

        var app = builder.Build();

        app
            .UseHttpsRedirection()
            .UseHsts()
            .UseAuthentication()
            .UseAuthorization()
            .UseConfigurator()
            .UseSwagger()
            .UseSwaggerUI()
            .UseRouting()
            .UseEndpoints(x => x.MapControllers());

        await app.RunAsync();
    }
}