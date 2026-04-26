using Anch.DependencyInjection;
using Anch.HierarchicalExpand;
using Anch.RelativePath.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;
using Anch.SecuritySystem.DiTests.DomainObjects;
using Anch.SecuritySystem.DiTests.Environment;
using Anch.SecuritySystem.DiTests.Rules;
using Anch.SecuritySystem.DiTests.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: AnchTestFramework<TestEnvironment>]

namespace Anch.SecuritySystem.DiTests.Environment;

public class TestEnvironment : ITestEnvironment
{
    public IServiceProvider BuildServiceProvider(IServiceCollection services) =>

        ServiceCollectionServiceExtensions.AddSingleton(services.AddSecuritySystem(settings =>

                    settings
                        .SetQueryableSource(sp => ServiceProviderServiceExtensions.GetRequiredService<TestQueryableSource>(sp))
                        .SetGenericRepository<TestGenericRepository>()
                        .SetDefaultCancellationTokenSource<XUnitDefaultCancellationTokenSource>()

                        .AddPermissionSystem<TestPermissionSystemFactory>()

                        .AddDomainSecurity<Employee>(b => b.SetView(ExampleSecurityOperation.EmployeeView)
                            .SetEdit(ExampleSecurityOperation.EmployeeEdit)
                            .SetPath(SecurityPath<Employee>.Create(v => v.BusinessUnit)))

                        .AddSecurityContext<BusinessUnit>(Guid.NewGuid(),
                            scb => scb.SetHierarchicalInfo(
                                bu => bu.Parent,
                                new AncestorLinkInfo<BusinessUnit, BusinessUnitDirectAncestorLink>(bu => bu.Ancestor,
                                    bu => bu.Child),
                                new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(bu => bu.Source,
                                    bu => bu.Target)))

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
                            new SecurityRoleInfo(Guid.NewGuid())
                                { Operations = [ExampleSecurityOperation.BusinessUnitView] })

                        .AddSecurityRole(
                            ExampleSecurityRole.TestKeyedRole,
                            new SecurityRoleInfo(Guid.NewGuid())
                            {
                                Restriction = SecurityPathRestriction.Create<Location>().Add<BusinessUnit>(key: "testKey")
                            })

                        .AddSecurityRole(SecurityRole.Administrator, new SecurityRoleInfo(Guid.NewGuid()))

                        .AddSecurityOperation(
                            ExampleSecurityOperation.BusinessUnitView,
                            new SecurityOperationInfo { CustomExpandType = HierarchicalExpandType.None }))

                .AddRelativeDomainPath((Employee employee) => employee), typeof(TestCheckboxConditionFactory<>))

            .AddSingleton<TestQueryableSource>()
            .AddSingleton<TestPermissionStorge>()
            .AddSingleton<BusinessUnitAncestorLinkSourceExecuteCounter>()

            .AddEnvironmentHook(EnvironmentHookType.After, sp =>
            {
                sp.GetRequiredService<TestQueryableSource>().Reset();
                sp.GetRequiredService<TestPermissionStorge>().Reset();
                sp.GetRequiredService<BusinessUnitAncestorLinkSourceExecuteCounter>().Count = 0;
            })

            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
}