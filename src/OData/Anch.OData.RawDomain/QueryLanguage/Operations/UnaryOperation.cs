namespace Anch.OData.Domain.QueryLanguage.Operations;

/// <summary>The unary operations of the query language.</summary>
public enum UnaryOperation
{
    /// <summary>
    /// (! a)
    /// </summary>
    Not,

    /// <summary>
    /// (+ a)
    /// </summary>
    Plus,

    /// <summary>
    /// (- a)
    /// </summary>
    Negate,
}
