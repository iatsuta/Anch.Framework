using ExampleApp.Infrastructure.Services;

using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class InitController(
    IEmptySchemaInitializer emptySchemaInitializer,
    ISharedTestDataInitializer sharedTestDataInitializer) : ControllerBase
{
    [HttpPost]
    public async Task TestInitialize(CancellationToken cancellationToken)
    {
        await emptySchemaInitializer.Initialize(cancellationToken);
        await sharedTestDataInitializer.Initialize(cancellationToken);
    }
}