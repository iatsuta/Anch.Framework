namespace Anch.Core.ExpressionEvaluate;

[Flags]
public enum LambdaCompileMode
{
    None = 0,

    InjectMaybe = 1,

    IgnoreStringCase = 2,

    OptimizeBooleanLogic = 4,

    All = InjectMaybe + IgnoreStringCase + OptimizeBooleanLogic
}