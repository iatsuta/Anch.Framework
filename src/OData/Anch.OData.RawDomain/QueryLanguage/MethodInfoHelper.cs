using System.Reflection;

namespace Anch.OData.Domain.QueryLanguage;

/// <summary>Recognizes the .NET methods that the query language supports.</summary>
public static class MethodInfoHelper
{
    public static readonly MethodInfo StringContainsMethod = new Func<string, bool>("".Contains).Method;

    public static readonly MethodInfo StringStartsWithMethod = new Func<string, bool>("".StartsWith).Method;

    public static readonly MethodInfo StringEndsWithMethod = new Func<string, bool>("".EndsWith).Method;


    public static readonly MethodInfo CollectionAnyEmptyMethod = new Func<IEnumerable<int>, bool>(Enumerable.Any).Method.GetGenericMethodDefinition();

    public static readonly MethodInfo CollectionAnyFilterMethod = new Func<IEnumerable<int>, Func<int, bool>, bool>(Enumerable.Any).Method.GetGenericMethodDefinition();

    public static readonly MethodInfo CollectionAllFilterMethod = new Func<IEnumerable<int>, Func<int, bool>, bool>(Enumerable.All).Method.GetGenericMethodDefinition();

    /// <summary>Maps a .NET method to the query language method.</summary>
    /// <param name="methodInfo">The method.</param>
    /// <returns>The method type, or <c>null</c> if the method is not supported.</returns>
    public static MethodExpressionType? GetMethodType(this MethodInfo methodInfo)
    {
        if (methodInfo is null) throw new ArgumentNullException(nameof(methodInfo));

        if (methodInfo == StringStartsWithMethod)
        {
            return MethodExpressionType.StringStartsWith;
        }

        if (methodInfo == StringContainsMethod)
        {
            return MethodExpressionType.StringContains;
        }

        if (methodInfo == StringEndsWithMethod)
        {
            return MethodExpressionType.StringEndsWith;
        }

        if (methodInfo.IsGenericMethod)
        {
            var genMethodInfo = methodInfo.GetGenericMethodDefinition();

            if (genMethodInfo == CollectionAnyEmptyMethod || genMethodInfo == CollectionAnyFilterMethod)
            {
                return MethodExpressionType.CollectionAny;
            }

            if (genMethodInfo == CollectionAllFilterMethod)
            {
                return MethodExpressionType.CollectionAll;
            }
        }

        return null;
    }
}
