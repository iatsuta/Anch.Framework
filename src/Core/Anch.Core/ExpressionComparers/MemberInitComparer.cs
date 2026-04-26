using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class MemberInitComparer(ExpressionComparer rootComparer) : ExpressionComparer<MemberInitExpression>
{
	private readonly MemberBindingComparer memberBindingComparer = new (rootComparer);

	protected override bool PureEquals(MemberInitExpression x, MemberInitExpression y)
    {
        return rootComparer.Equals(x.NewExpression, y.NewExpression) && x.Bindings.SequenceEqual(y.Bindings, this.memberBindingComparer);
    }
}