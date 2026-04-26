namespace Anch.Core.ExpressionEvaluate;

public interface IExpressionEvaluatorStorage
{
    IExpressionEvaluator GetForType(Type type);
}