using CommonFramework.DependencyInjection;
using CommonFramework.RelativePath.DependencyInjection;
using CommonFramework.Testing;

using HierarchicalExpand;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.DependencyInjection;
using SecuritySystem.DiTests.DomainObjects;
using SecuritySystem.DiTests.Environment;
using SecuritySystem.DiTests.Rules;
using SecuritySystem.DiTests.Services;

[assembly: CommonTestFramework<TestEnvironment>]

namespace SecuritySystem.DiTests.Environment;

public class TestEnvironment : ITestEnvironment
{
    public IServiceProvider BuildServiceProvider(IServiceCollection services) =>

        services.AddSecuritySystem(settings =>

                settings
                    .SetQueryableSource(sp => sp.GetRequiredService<TestQueryableSource>())
                    .SetGenericRepository<TestGenericRepository>()
                    .SetDefaultCancellationTokenSource<XUnitDefaultCancellationTokenSource>()

                    .AddPermissionSystem<TestPermissionSystemFactory>()

                    .AddDomainSecurity<Employee>(b => b.SetView(ExampleSecurityOperation.EmployeeView)
                        .SetEdit(ExampleSecurityOperation.EmployeeEdit)
                        .SetPath(SecurityPath<Employee>.Create(v => v.BusinessUnit)))

                    .AddSecurityContext<BusinessUnit>(Guid
                            .NewGuid(),
                        scb => scb.SetHierarchicalInfo(
                            bu => bu.Parent,
                            new AncestorLinkInfo<BusinessUnit, BusinessUnitDirectAncestorLink>(bu => bu.Ancestor, bu => bu.Child),
                            new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(bu => bu.Source, bu => bu.Target)))

                    .AddSecurityContext<Location>(Guid.NewGuid())

                    .AddSecurityRole(
                        ExampleSecurityRole.TestRole,
                        new SecurityRoleInfo(Guid.NewGuid())
                        {
                            Children = [ExampleSecurityRole.TestRole2],
                            Operations = [ExampleSecurityOperation.EmployeeView, ExampleSecurityOperation.EmployeeEdit]
                        })

                    .AddSecurityRole(
                        ExampleSecurityRole.TestRole2,
                        new SecurityRoleInfo(Guid.NewGuid()) { Children = [ExampleSecurityRole.TestRole3] })

                    .AddSecurityRole(
                        ExampleSecurityRole.TestRole3,
                        new SecurityRoleInfo(Guid.NewGuid()))

                    .AddSecurityRole(
                        ExampleSecurityRole.TestRole4,
                        new SecurityRoleInfo(Guid.NewGuid()) { Operations = [ExampleSecurityOperation.BusinessUnitView] })

                    .AddSecurityRole(
                        ExampleSecurityRole.TestKeyedRole,
                        new SecurityRoleInfo(Guid.NewGuid()) { Restriction = SecurityPathRestriction.Create<Location>().Add<BusinessUnit>(key: "testKey") })

                    .AddSecurityRole(SecurityRole.Administrator, new SecurityRoleInfo(Guid.NewGuid()))

                    .AddSecurityOperation(
                        ExampleSecurityOperation.BusinessUnitView,
                        new SecurityOperationInfo { CustomExpandType = HierarchicalExpandType.None }))

            .AddRelativeDomainPath((Employee employee) => employee)
            .AddSingleton(typeof(TestCheckboxConditionFactory<>))

            .AddSingleton<TestQueryableSource>()
            .AddSingleton<TestPermissionStorge>()
            .AddSingleton<BusinessUnitAncestorLinkSourceExecuteCounter>()

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });


    public ValueTask Cleanup(IServiceProvider serviceProvider, CancellationToken ct)
    {
        serviceProvider.GetRequiredService<TestQueryableSource>().Reset();
        serviceProvider.GetRequiredService<TestPermissionStorge>().Reset();
        serviceProvider.GetRequiredService<BusinessUnitAncestorLinkSourceExecuteCounter>().Count = 0;

        return ValueTask.CompletedTask;
    }
}