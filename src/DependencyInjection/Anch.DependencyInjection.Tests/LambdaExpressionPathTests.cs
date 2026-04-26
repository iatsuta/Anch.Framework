using System.Linq.Expressions;
using System.Reflection;

using Anch.Core;

namespace Anch.DependencyInjection.Tests;

public class LambdaExpressionPathTests
{
    [Fact]
    public void LambdaExpressionPath_ShouldBeEqual_ForDifferentInstancesWithSameExpressions()
    {
        // Arrange: create two arrays of identical expressions
        var array1 = new Expression<Func<int, int>>[] { x => x + 1, y => y - 2 };
        var array2 = new Expression<Func<int, int>>[] { x => x + 1, y => y - 2 };

        var path1 = new LambdaExpressionPath(array1);
        var path2 = new LambdaExpressionPath(array2);

        // Act & Assert
        Assert.Equivalent(path2, path1);

        // Check Equals
        Assert.True(path1.Equals(path2));

        // Check reference equality (should be different objects)
        Assert.False(ReferenceEquals(path1, path2));

        // Check hash code
        Assert.Equal(path2.GetHashCode(), path1.GetHashCode());
    }

    [Fact]
    public void Create_ShouldResolveProperties_ForPathWithCollection()
    {
        // arrange
        var path = "Items.SubItem.Value";

        // act
        var parsedPath = LambdaExpressionPath.Create(typeof(ObjA), path.Split('.'));

        // assert
        Assert.Equivalent(
            new[]
            {
                typeof(ObjA).GetRequiredProperty(nameof(ObjA.Items), BindingFlags.Public | BindingFlags.Instance),
                typeof(ObjB).GetRequiredProperty(nameof(ObjB.SubItem), BindingFlags.Public | BindingFlags.Instance),
                typeof(ObjC).GetRequiredProperty(nameof(ObjC.Value), BindingFlags.Public | BindingFlags.Instance)
            },
            parsedPath.Properties.Select(expr => expr.GetProperty()));
    }

    public record ObjA(IEnumerable<ObjB> Items);

    public record ObjB(ObjC SubItem);

    public record ObjC(int Value);
}