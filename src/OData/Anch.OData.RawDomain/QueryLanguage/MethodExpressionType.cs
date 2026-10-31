namespace Anch.OData.Domain.QueryLanguage;

/// <summary>The methods that the query language supports.</summary>
public enum MethodExpressionType
{
    /// <summary>The string contains a substring.</summary>
    StringContains,

    /// <summary>The string starts with a prefix.</summary>
    StringStartsWith,

    /// <summary>The string ends with a suffix.</summary>
    StringEndsWith,

    /// <summary>Some element of the collection (optionally one that satisfies a condition) exists.</summary>
    CollectionAny,

    /// <summary>Every element of the collection satisfies a condition.</summary>
    CollectionAll,
}
