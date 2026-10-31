using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

/// <summary>Base of the structural expression comparers: two expressions are equal when their node type, type and content are equal.</summary>
/// <typeparam name="TExpression">The compared expression node type.</typeparam>
public abstract class ExpressionComparer<TExpression> : IEqualityComparer<TExpression>
    where TExpression : Expression
{
    /// <inheritdoc/>
    public bool Equals(TExpression? x, TExpression? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return x.NodeType == y.NodeType && x.Type == y.Type && x.GetType() == y.GetType() && this.PureEquals(x, y);
    }

    /// <summary>Compares the content of two nodes of the same kind.</summary>
    /// <param name="x">The first node.</param>
    /// <param name="y">The second node.</param>
    protected abstract bool PureEquals(TExpression x, TExpression y);

    /// <inheritdoc/>
    public virtual int GetHashCode(TExpression obj)
    {
        return obj.Type.GetHashCode() ^ obj.NodeType.GetHashCode();
    }
}

/// <summary>
/// Compares expression trees by structure instead of by reference, so two lambdas that were written separately but look the same are equal.
/// Used as the equality of <c>PropertyAccessors</c>, <c>LambdaExpressionPath</c> and the OData and security rule records.
/// </summary>
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

    /// <summary>Comparer of lambda nodes.</summary>
    public virtual IEqualityComparer<LambdaExpression> LambdaComparer { get; }

    /// <summary>Comparer of parameter nodes.</summary>
    public virtual IEqualityComparer<ParameterExpression> ParameterComparer { get; }

    /// <summary>Comparer of method call nodes.</summary>
    public virtual IEqualityComparer<MethodCallExpression> MethodCallComparer { get; }

    /// <summary>Comparer of member access nodes.</summary>
    public virtual IEqualityComparer<MemberExpression> MemberComparer { get; }

    /// <summary>Comparer of binary nodes.</summary>
    public virtual IEqualityComparer<BinaryExpression> BinaryComparer { get; }

    /// <summary>Comparer of constant nodes.</summary>
    public virtual IEqualityComparer<ConstantExpression> ConstantComparer { get; }

    /// <summary>Comparer of unary nodes.</summary>
    public virtual IEqualityComparer<UnaryExpression> UnaryComparer { get; }

    /// <summary>Comparer of array creation nodes.</summary>
    public virtual IEqualityComparer<NewArrayExpression> NewArrayComparer { get; }

    /// <summary>Comparer of object creation nodes.</summary>
    public virtual IEqualityComparer<NewExpression> NewComparer { get; }

    /// <summary>Comparer of member initialization nodes.</summary>
    public virtual IEqualityComparer<MemberInitExpression> MemberInitComparer { get; }

    /// <summary>Comparer of conditional nodes.</summary>
    public virtual IEqualityComparer<ConditionalExpression> ConditionalComparer { get; }

    /// <summary>Comparer of list initialization nodes.</summary>
    public virtual IEqualityComparer<ListInitExpression> ListInitComparer { get; }


    /// <inheritdoc/>
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


    /// <summary>Compares everything, including constants by value.</summary>
    public static ExpressionComparer Default { get; } = new();

    /// <summary>Does not allow constants: use it for trees whose constants were replaced by parameters.</summary>
    public static ExpressionComparer WithoutConst { get; } = new WithoutConstExpressionComparer();

    private class WithoutConstExpressionComparer : ExpressionComparer
    {
        public override IEqualityComparer<ConstantExpression> ConstantComparer =>
            throw new InvalidOperationException($"{nameof(ConstantExpression)} not allowed");
    }
}