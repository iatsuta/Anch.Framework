namespace Anch.Core.ExpressionEvaluate;

/// <summary>Transformations applied to a lambda before it is compiled.</summary>
[Flags]
public enum LambdaCompileMode
{
    /// <summary>Compile as is.</summary>
    None = 0,

    /// <summary>Make member chains null-safe.</summary>
    InjectMaybe = 1,

    /// <summary>Compare strings ignoring case.</summary>
    IgnoreStringCase = 2,

    /// <summary>Simplify boolean logic with constants.</summary>
    OptimizeBooleanLogic = 4,

    /// <summary>All transformations.</summary>
    All = InjectMaybe + IgnoreStringCase + OptimizeBooleanLogic
}