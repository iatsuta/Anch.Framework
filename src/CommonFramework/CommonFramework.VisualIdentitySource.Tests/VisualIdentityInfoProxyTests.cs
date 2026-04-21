using CommonFramework.ExpressionComparers;
using CommonFramework.VisualIdentitySource.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.VisualIdentitySource.Tests;

public class VisualIdentityInfoProxyTests
{
    [Fact]
    public void Should_Resolve_VisualIdentityInfo_With_DefaultNameLambda()
    {
        //Arrange
        var nameLambda = ExpressionHelper.Create((TestObject1 v) => v.Name);

        var sp = new ServiceCollection()
            .AddVisualIdentitySource()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var expectedResult = nameLambda.GetProperty().ToGetLambdaExpression();

        //Act
        var visualIdentityInfo = sp.GetRequiredService<IVisualIdentityInfo<TestObject1>>();

        //Assert
        Assert.Equal(expectedResult, visualIdentityInfo.Name.Path, ExpressionComparer.Default);
    }

    [Fact]
    public void Should_Resolve_VisualIdentityInfo_With_CustomNameLambda()
    {
        //Arrange
        var nameLambda = ExpressionHelper.Create((TestObject2 v) => v.MyName);

        var sp = new ServiceCollection()
            .AddVisualIdentitySource(b => b.SetName(nameLambda))
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        //Act
        var visualIdentityInfo = sp.GetRequiredService<IVisualIdentityInfo<TestObject2>>();

        //Assert
        Assert.Equal(nameLambda, visualIdentityInfo.Name.Path, ExpressionComparer.Default);
    }

    public class TestObject1
    {
        public required string Name { get; set; }
    }

    public class TestObject2
    {
        public required string MyName { get; set; }
    }
}