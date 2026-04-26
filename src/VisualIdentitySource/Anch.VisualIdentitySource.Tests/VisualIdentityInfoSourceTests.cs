using Anch.Core;
using Anch.Core.ExpressionComparers;
using Anch.VisualIdentitySource.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.VisualIdentitySource.Tests;

public class VisualIdentityInfoSourceTests
{
    [Fact]
    public void GetDefaultVisualIdentityInfo_ResultCorrected()
    {
        //Arrange
        var nameLambda = ExpressionHelper.Create((TestObject1 v) => v.Name);

        var sp = new ServiceCollection()
            .AddVisualIdentitySource()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var service = sp.GetRequiredService<IVisualIdentityInfoSource>();

        var expectedResult = nameLambda.GetProperty().ToGetLambdaExpression();

        //Act
        var result = service.GetVisualIdentityInfo<TestObject1>();

        //Assert
        Assert.Equal(expectedResult, result.Name.Path, ExpressionComparer.Default);
    }

    [Fact]
    public void GetCustomVisualIdentityInfo_ResultCorrected()
    {
        //Arrange
        var nameLambda = ExpressionHelper.Create((TestObject2 v) => v.MyName);

        var sp = new ServiceCollection()
            .AddVisualIdentitySource(b => b.SetName(nameLambda))
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var service = sp.GetRequiredService<IVisualIdentityInfoSource>();

        var expectedResult = new VisualIdentityInfo<TestObject2>(nameLambda);

        //Act
        var result = service.GetVisualIdentityInfo<TestObject2>();

        //Assert
        Assert.Equal(expectedResult, result);
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