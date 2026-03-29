using CommonFramework;
using ExampleApp.Infrastructure.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class InitController([FromKeyedServices(RootAppInitializer.Key)] IInitializer rootInitializer) : ControllerBase
{
    [HttpPost]
    public Task TestInitialize(CancellationToken cancellationToken) => rootInitializer.Initialize(cancellationToken);
}