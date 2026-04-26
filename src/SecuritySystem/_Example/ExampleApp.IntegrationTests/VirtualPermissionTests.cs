using Anch.SecuritySystem.Testing;
using ExampleApp.Api.Controllers;
using ExampleApp.Domain;

namespace ExampleApp.IntegrationTests;

public abstract class VirtualPermissionTests(IServiceProvider rootServiceProvider) : TestBase(rootServiceProvider)
{
    [Theory]
    [AnchMemberData(nameof(Impersonate_LoadTestObjects_DataCorrected_Cases))]
    public async Task Impersonate_LoadTestObjects_DataCorrected(string runAs, string[] expectedBuList, CancellationToken ct)
    {
        // Arrange
        this.AuthManager.For(runAs).LoginAs();

        // Act
        var result = await this.GetEvaluator<TestController>().EvaluateAsync(TestingScopeMode.Read, async testController =>
        {
            return new
            {
                CurrentUserLogin = testController.GetCurrentUserLogin(),
                BuNames = (await testController.GetTestObjects(ct)).Select(v => v.BuName).ToList()
            };
        });

        // Assert
        Assert.Equal(runAs, result.CurrentUserLogin);

        var buNameList = result.BuNames.OrderBy(v => v).ToList();
        Assert.Equivalent(expectedBuList.OrderBy(v => v), buNameList);
    }

    public IEnumerable<object?[]> Impersonate_LoadTestObjects_DataCorrected_Cases()
    {
        yield return ["TestRootUser", new[] { $"Test{nameof(BusinessUnit)}1-Child", $"Test{nameof(BusinessUnit)}2-Child" }];
        yield return ["TestEmployee1", new[] { $"Test{nameof(BusinessUnit)}1-Child" }];
        yield return ["TestEmployee2", new[] { $"Test{nameof(BusinessUnit)}2-Child" }];
    }

    [Theory]
    [AnchMemberData(nameof(Impersonate_LoadBuByAncestorView_DataCorrected_Cases))]
    public async Task Impersonate_LoadBuByAncestorView_DataCorrected(string runAs, string[] expectedBuList, CancellationToken ct)
    {
        // Arrange
        this.AuthManager.For(runAs).LoginAs();

        // Act
        var result = await this.GetEvaluator<TestController>().EvaluateAsync(TestingScopeMode.Read, async testController =>
        {
            return new
            {
                CurrentUserLogin = testController.GetCurrentUserLogin(),
                BuNames = (await testController.GetBuList(ct)).Select(v => v.Name).ToList()
            };
        });

        // Assert
        Assert.Equal(runAs, result.CurrentUserLogin);

        var buNameList = result.BuNames.OrderBy(v => v).ToList();
        Assert.Equivalent(expectedBuList.OrderBy(v => v), buNameList);
    }

    public IEnumerable<object?[]> Impersonate_LoadBuByAncestorView_DataCorrected_Cases()
    {
        var rootBu = "TestRootBu";

        var bu_1 = $"Test{nameof(BusinessUnit)}1";
        var bu_1_1 = $"Test{nameof(BusinessUnit)}1-Child";

        var bu_2 = $"Test{nameof(BusinessUnit)}2";
        var bu_2_1 = $"Test{nameof(BusinessUnit)}2-Child";


        yield return ["TestRootUser", new[] { rootBu, bu_1, bu_1_1, bu_2, bu_2_1 }];
        yield return ["TestEmployee1", new[] { rootBu, bu_1, bu_1_1 }];
        yield return ["TestEmployee2", new[] { rootBu, bu_2, bu_2_1 }];
    }
}