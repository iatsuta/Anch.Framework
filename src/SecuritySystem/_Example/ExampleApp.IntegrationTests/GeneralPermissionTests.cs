using Anch.GenericQueryable;
using Anch.GenericRepository;
using Anch.SecuritySystem;
using Anch.SecuritySystem.AvailableSecurity;
using Anch.SecuritySystem.DomainServices;
using Anch.Testing.Xunit;

using ExampleApp.Application;
using ExampleApp.Domain;

using Microsoft.Extensions.DependencyInjection;

namespace ExampleApp.IntegrationTests;

public abstract class GeneralPermissionTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [AnchFact]
    public async Task SetRoleAsync_ShouldPreservePermissionIdentity(CancellationToken ct)
    {
        // Arrange
        var testPermission = new TestPermission(ExampleSecurityRole.DefaultRole) { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()) };

        // Act
        var principalIdentity = await this.AuthManager.For("TestPrincipal").SetRoleAsync(testPermission, ct);

        // Assert
        var managedPrincipal = await this.AuthManager.For(principalIdentity).GetPrincipalAsync(ct);

        var managedPermission = Assert.Single(managedPrincipal.Permissions);

        Assert.Equal(testPermission.Identity, managedPermission.Identity);
    }

    [AnchFact]
    public async Task AssignGeneralPermission_PermissionResolved(CancellationToken ct)
    {
        // Arrange
        var principalName = "TestPrincipal";

        var buIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>("TestRootBu", ct);

        var testRole = ExampleSecurityRole.BuManager;

        var testPermission = new TestPermission(testRole) { BusinessUnit = buIdentity };

        var principalIdentity = await this.AuthManager.For(principalName).SetRoleAsync(testPermission, ct);
        this.AuthManager.For(principalIdentity).LoginAs();

        // Act
        var availableSecurityRoles = await this.GetEvaluator<IAvailableSecurityRoleSource>().EvaluateAsync(TestingScopeMode.Read,
            async availableSecurityRoleSource => await availableSecurityRoleSource.GetAvailableSecurityRoles().ToArrayAsync(ct));

        // Assert
        Assert.Equivalent(new[] { testRole }, availableSecurityRoles);

        var managedPrincipal = await this.AuthManager.For(principalName).GetPrincipalAsync(ct);

        Assert.Equal(principalIdentity, managedPrincipal.Header.Identity);
        Assert.Equal(principalName, managedPrincipal.Header.Name);
        Assert.False(managedPrincipal.Header.IsVirtual);

        var managedPermission = Assert.Single(managedPrincipal.Permissions);

        Assert.False(managedPermission.IsVirtual);
        Assert.Equivalent(testPermission.Restrictions, managedPermission.Restrictions);
    }

    [AnchFact]
    public async Task AssignGeneralPermission_WithRootBu_AllTestObjectsResolved(CancellationToken ct)
    {
        // Arrange
        var buIdentity = await this.AuthManager.GetSecurityContextIdentityAsync<BusinessUnit, Guid>("TestRootBu", ct);

        var testRole = ExampleSecurityRole.BuManager;

        var testPermission = new TestPermission(testRole) { BusinessUnit = buIdentity };

        var principalId = await this.AuthManager.For("TestPrincipal").SetRoleAsync([testPermission, ExampleSecurityRole.DefaultRole], ct);
        this.AuthManager.For(principalId).LoginAs();

        // Act

        // Assert
        await this.GetEvaluator<IServiceProvider>().EvaluateAsync(TestingScopeMode.Read, async sp =>
        {
            var testObjectDomainSecurityService = sp.GetRequiredService<IDomainSecurityService<TestObject>>();
            var securityProvider = testObjectDomainSecurityService.GetSecurityProvider(testRole);

            var queryableSource = sp.GetRequiredService<IQueryableSource>();

            var testObjectRepositoryFactory = sp.GetRequiredService<IRepositoryFactory<TestObject>>();
            var testObjectRepository = testObjectRepositoryFactory.Create(testRole);

            var expectedResult = await queryableSource.GetQueryable<TestObject>().GenericToListAsync(ct);

            var result = await testObjectRepository.GetQueryable().GenericToListAsync(ct);

            Assert.Equivalent(expectedResult.OrderBy(v => v.Id), result.OrderBy(v => v.Id));

            foreach (var testObject in result)
            {
                Assert.True(await securityProvider.HasAccessAsync(testObject, ct));
            }
        });
    }
}