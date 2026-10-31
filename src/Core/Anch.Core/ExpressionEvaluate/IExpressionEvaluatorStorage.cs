namespace Anch.Core.ExpressionEvaluate;

/// <summary>Provides an expression evaluator (with its own compile cache) per owner type.</summary>
public interface IExpressionEvaluatorStorage
{
    /// <summary>Returns the evaluator for a type.</summary>
    /// <param name="type">The owner type.</param>
    IExpressionEvaluator GetForType(Type type);
}