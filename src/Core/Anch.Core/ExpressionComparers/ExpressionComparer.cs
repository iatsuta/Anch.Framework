using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public abstract class ExpressionComparer<TExpression> : IEqualityComparer<TExpression>
	where TExpression : Expression
{
    public bool Equals(TExpression? x, TExpression? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return x.NodeType == y.NodeType && x.Type == y.Type && x.GetType() == y.GetType() && this.PureEquals(x, y);
	}

	protected abstract bool PureEquals(TExpression x, TExpression y);

    public virtual int GetHashCode(TExpression obj)
    {
        return obj.Type.GetHashCode() ^ obj.NodeType.GetHashCode();
    }
}

public class ExpressionComparer : ExpressionComparer<Expression>
{
	protected ExpressionComparer()
	{
		this.LambdaComparer = new LambdaComparer(this);
		this.ParameterComparer = new ParameterComparer();
		this.MethodCallComparer = new MethodCallComparer(this);
		this.BinaryComparer = new BinaryComparer(this);
		this.ConstantComparer = new ConstantComparer();
		this.UnaryComparer = new UnaryComparer(this);
		this.NewArrayComparer = new NewArrayComparer(this);
		this.NewComparer = new NewComparer(this);
		this.MemberComparer = new MemberComparer(this);
		this.MemberInitComparer = new MemberInitComparer(this);
		this.ConditionalComparer = new ConditionalComparer(this);
		this.ListInitComparer = new ListInitComparer(this);
	}

	public virtual IEqualityComparer<LambdaExpression> LambdaComparer { get; }

	public virtual IEqualityComparer<ParameterExpression> ParameterComparer { get; }

	public virtual IEqualityComparer<MethodCallExpression> MethodCallComparer { get; }

	public virtual IEqualityComparer<MemberExpression> MemberComparer { get; }

	public virtual IEqualityComparer<BinaryExpression> BinaryComparer { get; }

	public virtual IEqualityComparer<ConstantExpression> ConstantComparer { get; }

	public virtual IEqualityComparer<UnaryExpression> UnaryComparer { get; }

	public virtual IEqualityComparer<NewArrayExpression> NewArrayComparer { get; }

	public virtual IEqualityComparer<NewExpression> NewComparer { get; }

	public virtual IEqualityComparer<MemberInitExpression> MemberInitComparer { get; }

	public virtual IEqualityComparer<ConditionalExpression> ConditionalComparer { get; }

	public virtual IEqualityComparer<ListInitExpression> ListInitComparer { get; }


	protected override bool PureEquals(Expression x, Expression y)
	{
		return (x, y) switch
		{
			(LambdaExpression xx, LambdaExpression yy) => this.LambdaComparer.Equals(xx, yy),
			(ParameterExpression xx, ParameterExpression yy) => this.ParameterComparer.Equals(xx, yy),
			(MethodCallExpression xx, MethodCallExpression yy) => this.MethodCallComparer.Equals(xx, yy),
			(MemberExpression xx, MemberExpression yy) => this.MemberComparer.Equals(xx, yy),
			(BinaryExpression xx, BinaryExpression yy) => this.BinaryComparer.Equals(xx, yy),
			(ConstantExpression xx, ConstantExpression yy) => this.ConstantComparer.Equals(xx, yy),
			(UnaryExpression xx, UnaryExpression yy) => this.UnaryComparer.Equals(xx, yy),
			(NewArrayExpression xx, NewArrayExpression yy) => this.NewArrayComparer.Equals(xx, yy),
			(NewExpression xx, NewExpression yy) => this.NewComparer.Equals(xx, yy),
			(MemberInitExpression xx, MemberInitExpression yy) => this.MemberInitComparer.Equals(xx, yy),
			(ConditionalExpression xx, ConditionalExpression yy) => this.ConditionalComparer.Equals(xx, yy),
			(ListInitExpression xx, ListInitExpression yy) => this.ListInitComparer.Equals(xx, yy),
			_ => throw new NotImplementedException(
				$"Comparison between expression types '{x?.GetType().Name ?? "null"}' and '{y?.GetType().Name ?? "null"}' is not implemented.")
		};
	}


	public static ExpressionComparer Default { get; } = new();

	public static ExpressionComparer WithoutConst { get; } = new WithoutConstExpressionComparer();

	private class WithoutConstExpressionComparer : ExpressionComparer
	{
		public override IEqualityComparer<ConstantExpression> ConstantComparer =>
			throw new InvalidOperationException($"{nameof(ConstantExpression)} not allowed");
	}
}