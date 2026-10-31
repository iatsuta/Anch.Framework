namespace Anch.OData.Domain.QueryLanguage;

/// <summary>Extensions for <see cref="MethodExpressionType"/>.</summary>
public static class MethodExpressionTypeExtensions
{
    /// <summary>Returns the method name as written in a query.</summary>
    /// <param name="type">The method.</param>
    public static string ToFormatString(this MethodExpressionType type) =>
        type switch
        {
            MethodExpressionType.StringStartsWith => "StartsWith",

            MethodExpressionType.StringContains => "Contains",

            MethodExpressionType.StringEndsWith => "EndsWith",

            MethodExpressionType.CollectionAny => "Any",

            MethodExpressionType.CollectionAll => "All",

            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
}
