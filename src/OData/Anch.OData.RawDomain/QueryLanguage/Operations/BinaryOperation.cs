namespace Anch.OData.Domain.QueryLanguage.Operations;

/// <summary>The binary operations of the query language.</summary>
public enum BinaryOperation
{
    /// <summary>Equality (<c>==</c>).</summary>
    Equal,

    /// <summary>Greater than or equal (<c>&gt;=</c>).</summary>
    GreaterThanOrEqual,

    /// <summary>Less than or equal (<c>&lt;=</c>).</summary>
    LessThanOrEqual,

    /// <summary>Inequality (<c>!=</c>).</summary>
    NotEqual,

    /// <summary>Greater than (<c>&gt;</c>).</summary>
    GreaterThan,

    /// <summary>Less than (<c>&lt;</c>).</summary>
    LessThan,

    /// <summary>Addition (<c>+</c>).</summary>
    Add,

    /// <summary>Subtraction (<c>-</c>).</summary>
    Subtract,

    /// <summary>Multiplication (<c>*</c>).</summary>
    Mul,

    /// <summary>Division (<c>/</c>).</summary>
    Div,

    /// <summary>Remainder (<c>%</c>).</summary>
    Mod,

    /// <summary>Logical or (<c>||</c>).</summary>
    OrElse,

    /// <summary>Logical and (<c>&amp;&amp;</c>).</summary>
    AndAlso,

    ///// <summary>
    ///// If added to the parser, associativity must be taken into account
    ///// </summary>
    //Pow,
}
