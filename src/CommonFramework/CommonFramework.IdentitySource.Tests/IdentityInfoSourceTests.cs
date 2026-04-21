using CommonFramework.ExpressionComparers;
using CommonFramework.IdentitySource.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace CommonFramework.IdentitySource.Tests;

public class IdentityInfoSourceTests
{
    [Fact]
    public void GetDefaultIdentityInfo_ResultCorrected()
    {
        //Arrange
        var idLambda = ExpressionHelper.Create((TestObject1 v) => v.Id);

        var sp = new ServiceCollection()
            .AddIdentitySource()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var service = sp.GetRequiredService<IIdentityInfoSource>();

        var expectedResult = idLambda.GetProperty().ToGetLambdaExpression();

        //Act
        var result = service.GetIdentityInfo<TestObject1, int>();

        //Assert
        Assert.Equal(expectedResult, result.Id.Path, ExpressionComparer.Default);
    }


    [Fact]
    public void GetCustomIdentityInfo_ResultCorrected()
    {
        //Arrange
        var idLambda = ExpressionHelper.Create((TestObject2 v) => v.MyId);

        var sp = new ServiceCollection()
            .AddIdentitySource(b => b.SetId(idLambda))
            .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

        var service = sp.GetRequiredService<IIdentityInfoSource>();

        var expectedResult = new IdentityInfo<TestObject2, Guid>(idLambda);

        //Act
        var result = service.GetIdentityInfo(typeof(TestObject2));

        //Assert
        Assert.Equal(expectedResult, result);
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