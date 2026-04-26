namespace Anch.OData.Tests;

public class MainTest : TestBase
{
    [Fact]
    public void TestODataParse()
    {
        // Arrange
        //var request = File.ReadAllText("request.odata");

        var request = "$top=30&$filter=Active eq true and (ManagementUnit/Id eq guid'01f5403e-a653-4752-a729-ba5164890d7a')";

        // Act
        var result = this.RawSelectOperationParser.Parse(request);

        // Assert

        //throw new NotImplementedException();

        //var r = result zzz.Filter.ToString();
    }
}