using ExampleApp.Infrastructure.Services;

using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class InitController(
    IEmptySchemaInitializer emptySchemaInitializer,
    ITestDataInitializer testDataInitializer) : ControllerBase
{
    [HttpPost]
    public async Task TestInitialize(CancellationToken ct)
    {
        await emptySchemaInitializer.Initialize(ct);
        await testDataInitializer.Initialize(ct);
    }
}