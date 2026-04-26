namespace Anch.OData.Tests;

public class DateParsingTests : TestBase
{
    private readonly IQueryable<TestClass> stream = new[]
    {
        new TestClass
        {
            Id = 1,
            StartDateNotNull = new DateTime(2018, 8, 1),
            EndDateNull = new DateTime(2018, 8, 1)
        },
        new TestClass
        {
            Id = 2,
            StartDateNotNull = new DateTime(2018, 8, 2),
            EndDateNull = new DateTime(2018, 8, 2),
        },
        new TestClass
        {
            Id = 3,
            StartDateNotNull = new DateTime(2018, 8, 3),
            EndDateNull = null,
        },
        new TestClass
        {
            Id = 4,
            StartDateNotNull = new DateTime(2018, 9, 10),
            EndDateNull = new DateTime(2018, 9, 10),
        }
    }.AsQueryable();

    [Fact]
    public void NotNullableDate__Month_Equal_Value_ElementFounded()
    {
        // Arrange
        var query = "$top=70&$filter=month(startDateNotNull) eq 9&$orderby=startDateNotNull";

        // Act
        var res = this.ParseAndProcess(query);

        // Arrange
        Assert.Equal(4, res.Id);
    }

    [Fact]
    public void NotNullableDate__Day_Equal_Value_ElementFounded()
    {
        // Arrange
        var query = "$top=70&$filter=day(StartDateNotNull) eq 10";

        // Act
        var res = this.ParseAndProcess(query);

        // Arrange
        Assert.Equal(4, res.Id);
    }

    [Fact]
    public void NotNullableDate_Equal_Value_ElementFounded()
    {
        // Arrange
        var query = "$top=70&$filter=StartDateNotNull eq datetime'2018-08-01'";

        // Act
        var res = this.ParseAndProcess(query);

        // Arrange
        Assert.Equal(1, res.Id);
    }

    [Fact]
    public void NotNullableDate_Equal_Null_Exception()
    {
        // Arrange
        var query = "$top=70&$filter=StartDateNotNull eq null";

        // Act
        var res = Assert.Throws<InvalidOperationException>(() => this.ParseAndProcess(query));

        // Arrange
        Assert.Equal("Sequence contains no elements", res.Message);
    }

    [Fact]
    public void NullableDate_Equal_Null_ElementFounded()
    {
        // Arrange
        var query = "$top=70&$filter=EndDateNull eq null";

        // Act
        var res = this.ParseAndProcess(query);

        // Arrange
        Assert.Equal(3, res.Id);
    }

    [Fact]
    public void NullableDate_Equal_Value_Null_ElementFounded()
    {
        // Arrange
        var query = "$top=70&$filter=EndDateNull eq datetime'2018-08-01'";

        // Act
        var res = this.ParseAndProcess(query);

        // Arrange
        Assert.Equal(1, res.Id);
    }

    private TestClass ParseAndProcess(string query)
    {
        var selectOperationGeneric = this.SelectOperationParser.Parse<TestClass>(query);

        var res = selectOperationGeneric.Inject(this.stream).Single();

        return res;
    }

    public class TestClass
    {
        public int Id { get; set; }

        public DateTime StartDateNotNull { get; set; }

        public DateTime? EndDateNull { get; set; }
    }
}
