namespace Anch.OData.Tests.LiftTestData;

public class TestPeriodObjContainer
{
    public TestPeriod? Period => this.InnerObj?.Period;

    public TestPeriodObj? InnerObj { get; set; }
}
