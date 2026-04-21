using System.Linq.Expressions;

using CommonFramework.ExpressionComparers;

namespace CommonFramework.DependencyInjection.Tests;

public class DeepEqualsCollectionTests
{
    [Fact]
    public void Equals_ShouldReturnTrue_ForDifferentInstancesWithSameElements()
    {
        // Arrange
        var array1 = new[] { "a", "b", "c" };
        var array2 = new[] { "a", "b", "c" };

        var col1 = new DeepEqualsCollection<string>(array1);
        var col2 = new DeepEqualsCollection<string>(array2);

        // Act & Assert
        Assert.True(col1.Equals(col2));
        Assert.True(col1 == col2);
    }

    [Fact]
    public void GetHashCode_ShouldBeEqual_ForDifferentInstancesWithSameElements()
    {
        // Arrange
        var array1 = new[] { "x", "y", "z" };
        var array2 = new[] { "x", "y", "z" };

        var col1 = new DeepEqualsCollection<string>(array1);
        var col2 = new DeepEqualsCollection<string>(array2);

        // Act
        var hash1 = col1.GetHashCode();
        var hash2 = col2.GetHashCode();

        // Assert
        Assert.Equal(hash2, hash1);
    }

    [Fact]
    public void DeepEqualsCollection_ShouldBeEquivalent_ForDifferentInstancesWithSameExpressions()
    {
        // Arrange
        var array1 = new Expression<Func<int, int>>[] { x => x + 1, y => y - 2 };
        var array2 = new Expression<Func<int, int>>[] { x => x + 1, y => y - 2 };

        var col1 = DeepEqualsCollection.Create(array1, ExpressionComparer.Default);
        var col2 = DeepEqualsCollection.Create(array2, ExpressionComparer.Default);

        // Act;

        // Assert
        Assert.Equivalent(col2, col1);
    }
}