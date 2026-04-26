using Anch.Core;
using Anch.Core.ExpressionComparers;
using Anch.IdentitySource.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.IdentitySource.Tests;

public class IdentityInfoProxyTests
{
    [Fact]
    public void Should_Resolve_IdentityInfo_With_DefaultIdLambda()
    {
        //Arrange
        var idLambda = ExpressionHelper.Create((TestObject1 v) => v.Id);

        var sp = new ServiceCollection()
            .AddIdentitySource()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var expectedResult = idLambda.GetProperty().ToGetLambdaExpression();

        //Act
        var identityInfo = sp.GetRequiredService<IIdentityInfo<TestObject1, int>>();

        //Assert
        Assert.Equal(expectedResult, identityInfo.Id.Path, ExpressionComparer.Default);
    }

    [Fact]
    public void Should_Resolve_IdentityInfo_With_CustomIdLambda()
    {
        //Arrange
        var idLambda = ExpressionHelper.Create((TestObject2 v) => v.MyId);

        var sp = new ServiceCollection()
            .AddIdentitySource(b => b.SetId(idLambda))
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        //Act
        var identityInfo = sp.GetRequiredService<IIdentityInfo<TestObject2, Guid>>();

        //Assert
        Assert.Equal(idLambda, identityInfo.Id.Path, ExpressionComparer.Default);
    }

    public class TestObject1
    {
        public required int Id { get; set; }
    }

    public class TestObject2
    {
        public required Guid MyId { get; set; }
    }
}