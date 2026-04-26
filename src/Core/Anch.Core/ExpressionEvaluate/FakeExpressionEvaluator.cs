using System.Linq.Expressions;

namespace Anch.Core.ExpressionEvaluate;

public class FakeExpressionEvaluator : IExpressionEvaluator
{
    public TDelegate Compile<TDelegate>(Expression<TDelegate> expression)
    {
        throw new NotImplementedException();
    }
}