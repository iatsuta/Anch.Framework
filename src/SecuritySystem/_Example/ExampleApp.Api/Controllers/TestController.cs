using Anch.GenericQueryable;
using Anch.SecuritySystem;
using Anch.SecuritySystem.UserSource;

using ExampleApp.Application;
using ExampleApp.Domain;

using Microsoft.AspNetCore.Mvc;

namespace ExampleApp.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class TestController(
    ICurrentUserSource<Employee> currentEmployeeSource,
    IRepositoryFactory<TestObject> testObjectRepositoryFactory,
    IRepositoryFactory<BusinessUnit> buRepositoryFactory,
    IRepositoryFactory<Employee> employeeRepositoryFactory) : ControllerBase
{
    [HttpGet]
    public async Task<List<TestObjectDto>> GetTestObjects(CancellationToken ct)
    {
        return await testObjectRepositoryFactory
            .Create(SecurityRule.View)
            .GetQueryable()
            .Select(testObj => new TestObjectDto(testObj.Id, testObj.BusinessUnit.Name))
            .GenericToListAsync(ct);
    }

    [HttpGet]
    public string GetCurrentUserLogin()
    {
        return currentEmployeeSource.CurrentUser.Login;
    }

    [HttpGet]
    public async Task<string> GetCurrentUserLoginByEmployee(CancellationToken ct)
    {
        return await employeeRepositoryFactory.Create(SecurityRule.View)
            .GetQueryable()
            .Select(employee => employee.Login)
            .GenericSingleAsync(ct);
    }

    [HttpGet]
    public async Task<List<BuDto>> GetBuList(CancellationToken ct)
    {
        return await buRepositoryFactory
            .Create(SecurityRule.View)
            .GetQueryable()
            .Select(bu => new BuDto(bu.Id, bu.Name, bu.Parent == null ? null : bu.Parent.Id))
            .GenericToListAsync(ct);
    }
}