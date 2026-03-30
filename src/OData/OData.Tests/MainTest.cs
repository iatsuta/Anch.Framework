namespace OData.Tests;

public class MainTest : TestBase
{
    [Fact]
    public void TestODataParse()
    {
        // Arrange
        var request = File.ReadAllText("request.odata");

        // Act
        var result = this.RawSelectOperationParser.Parse(request);

        // Assert

        throw new NotImplementedException();

        //var r = result zzz.Filter.ToString();
    }
}