using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class ListInitComparer(ExpressionComparer rootComparer) : ExpressionComparer<ListInitExpression>
{
	private readonly ElementInitComparer elementInitComparer = new(rootComparer);

	protected override bool PureEquals(ListInitExpression x, ListInitExpression y)
	{
		return rootComparer.Equals(x.NewExpression, y.NewExpression)
		       && this.CompareElementInitList(x.Initializers, y.Initializers);
	}

	private bool CompareElementInitList(ReadOnlyCollection<ElementInit> x, ReadOnlyCollection<ElementInit> y)
	{
		return x == y || x.SequenceEqual(y, this.elementInitComparer);
	}
}