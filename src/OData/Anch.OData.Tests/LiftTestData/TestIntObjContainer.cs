namespace Anch.OData.Tests.LiftTestData;

public class TestIntObjContainer
{
    public int? Int => this.InnerObj?.Int;

    public TestIntObj? InnerObj { get; set; }
}
