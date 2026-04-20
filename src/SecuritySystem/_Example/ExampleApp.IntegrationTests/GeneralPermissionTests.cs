using CommonFramework.GenericRepository;
using CommonFramework.Testing;

using ExampleApp.Application;
using ExampleApp.Domain;

using GenericQueryable;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem;
using SecuritySystem.AvailableSecurity;
using SecuritySystem.DomainServices;

namespace ExampleApp.IntegrationTests;

public abstract class GeneralPermissionTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [CommonFact]
    public async Task SetRoleAsync_ShouldPreservePermissionIdentity(CancellationToken ct)
    {
        // Arrange
        var testPermission = new TestPermission(ExampleSecurityRole.DefaultRole) { Identity = TypedSecurityIdentity.Create(Guid.NewGuid()) };

        // Act
        var principalIdentity = await this.AuthManager.For("TestPrincipal").SetRoleAsync(testPermission, ct);

        // Assert
        var managedPrincipal = await this.AuthManager.For(principalIdentity).GetPrincipalAsync(ct);

        var managedPermission = managedPrincipal.Permissions.Should().ContainSingle().Subject;

        managedPermission.Identity.Should().Be(testPermission.Identity);
    }

    [CommonFact]
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
        availableSecurityRoles.Should().BeEquivalentTo([testRole]);

        var managedPrincipal = await this.AuthManager.For(principalName).GetPrincipalAsync(ct);

        managedPrincipal.Header.Identity.Should().Be(principalIdentity);
        managedPrincipal.Header.Name.Should().Be(principalName);
        managedPrincipal.Header.IsVirtual.Should().Be(false);

        var managedPermission = managedPrincipal.Permissions.Should().ContainSingle().Subject;

        managedPermission.IsVirtual.Should().Be(false);
        managedPermission.Restrictions.Should().BeEquivalentTo(testPermission.Restrictions);
    }

    [CommonFact]
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

            result.OrderBy(v => v.Id).Should().BeEquivalentTo(expectedResult.OrderBy(v => v.Id));

            foreach (var testObject in result)
            {
                (await securityProvider.HasAccessAsync(testObject, ct)).Should().Be(true);
            }
        });
    }
}