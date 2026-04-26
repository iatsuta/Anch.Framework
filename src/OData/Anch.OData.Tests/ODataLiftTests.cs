using Anch.OData.Tests.LiftTestData;

namespace Anch.OData.Tests;

public class ODataLiftTests : TestBase
{
    [Fact]
    public void NullableIntWithIntersect_Executed()
    {
        // Arrange
        var value = 100;

        var testQuery = $"$filter={value} ge {nameof(TestIntObjContainer.Int)}";

        var testData = new[] { new TestIntObjContainer { InnerObj = new TestIntObj { Int = value - 1 } }, new TestIntObjContainer { InnerObj = null } }
            .AsQueryable();

        // Act
        var typesSelectOperation = this.SelectOperationParser.Parse<TestIntObjContainer>(testQuery);

        // Assert
        var testResult = typesSelectOperation.Inject(testData).ToList();

        return;
    }
}