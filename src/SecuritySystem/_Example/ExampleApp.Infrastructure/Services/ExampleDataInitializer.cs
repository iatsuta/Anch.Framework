using Anch.Core;
using Anch.Core.Auth;
using Anch.GenericRepository;

using ExampleApp.Domain;
using ExampleApp.Domain.Auth.Virtual;

using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.Infrastructure.Services;

public class ExampleDataInitializer([FromKeyedServices(ICurrentUser.RawKey)]ICurrentUser rawCurrentUser, IGenericRepository genericRepository) : IInitializer
{
    public const string Key = "ExampleData";

    public async Task Initialize(CancellationToken cancellationToken)
    {
        {
            var currentEmployee = new Employee { Login = rawCurrentUser.Name };
            await genericRepository.SaveAsync(currentEmployee, cancellationToken);

            var currentEmployeeAdminVirtualPermission = new Administrator { Employee = currentEmployee };
            await genericRepository.SaveAsync(currentEmployeeAdminVirtualPermission, cancellationToken);
        }

        var testRootBu = new BusinessUnit { Name = "TestRootBu" };
        await genericRepository.SaveAsync(testRootBu, cancellationToken);

        var testRootMu = new ManagementUnit { Name = "TestRootMu" };
        await genericRepository.SaveAsync(testRootMu, cancellationToken);

        foreach (var index in Enumerable.Range(1, 2))
        {
            var testLocation = new Location { Name = $"Test{nameof(Location)}{index}", MyId = index };
            await genericRepository.SaveAsync(testLocation, cancellationToken);

            var testMiddleBu = new BusinessUnit { Name = $"Test{nameof(BusinessUnit)}{index}", Parent = testRootBu };
            await genericRepository.SaveAsync(testMiddleBu, cancellationToken);

            var testChildBu = new BusinessUnit { Name = $"Test{nameof(BusinessUnit)}{index}-Child", Parent = testMiddleBu };
            await genericRepository.SaveAsync(testChildBu, cancellationToken);

            var testMiddleMu = new ManagementUnit { Name = $"Test{nameof(ManagementUnit)}{index}", Parent = testRootMu };
            await genericRepository.SaveAsync(testMiddleMu, cancellationToken);

            var testChildMu = new ManagementUnit { Name = $"Test{nameof(ManagementUnit)}{index}-Child", Parent = testMiddleMu };
            await genericRepository.SaveAsync(testChildMu, cancellationToken);

            var testObj = new TestObject { BusinessUnit = testChildBu, ManagementUnit = testChildMu, Location = testLocation };
            await genericRepository.SaveAsync(testObj, cancellationToken);

            var testEmployee = new Employee { Login = $"Test{nameof(Employee)}{index}" };
            await genericRepository.SaveAsync(testEmployee, cancellationToken);

            var testVirtualPermission = new TestManager { BusinessUnit = testMiddleBu, Employee = testEmployee, Location = testLocation };
            await genericRepository.SaveAsync(testVirtualPermission, cancellationToken);
        }
    }
}