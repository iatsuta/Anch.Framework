using System.Collections.Immutable;
using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionComparers;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>
/// Контекстное правило доступа (лямбды)
/// </summary>
/// <typeparam name="TDomainObject"></typeparam>
public abstract record SecurityPath<TDomainObject>
{
    public abstract ImmutableArray<Type> UsedSecurityContextTypes { get; }

    public abstract SecurityPath<TNewDomainObject> OverrideInput<TNewDomainObject>(
        Expression<Func<TNewDomainObject, TDomainObject>> selector);

    public static SecurityPath<TDomainObject> Empty { get; } = Condition(_ => true);

    #region Create

    public SecurityPath<TDomainObject> And(SecurityPath<TDomainObject> other) => new AndSecurityPath(this, other);

    public SecurityPath<TDomainObject> And(Expression<Func<TDomainObject, bool>> securityFilter) => this.And(Condition(securityFilter));

    public SecurityPath<TDomainObject> And<TSecurityContext>(
        Expression<Func<TDomainObject, TSecurityContext?>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        this.And(Create(securityPath, required, key));

    public SecurityPath<TDomainObject> And<TSecurityContext>(
        Expression<Func<TDomainObject, IEnumerable<TSecurityContext>>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        this.And(Create(securityPath, required, key));

    public SecurityPath<TDomainObject> Or(SecurityPath<TDomainObject> other) => new OrSecurityPath(this, other);

    public SecurityPath<TDomainObject> Or(Expression<Func<TDomainObject, bool>> securityFilter) => this.Or(Condition(securityFilter));

    public SecurityPath<TDomainObject> Or<TSecurityContext>(
        Expression<Func<TDomainObject, TSecurityContext?>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        this.Or(Create(securityPath, required, key));

    public SecurityPath<TDomainObject> Or<TSecurityContext>(
        Expression<Func<TDomainObject, IEnumerable<TSecurityContext>>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        this.Or(Create(securityPath, required, key));

    public static SecurityPath<TDomainObject> Condition(Expression<Func<TDomainObject, bool>> securityFilter) =>
        new ConditionPath(securityFilter);

    public static SecurityPath<TDomainObject> Create<TSecurityContext>(
        Expression<Func<TDomainObject, TSecurityContext?>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        new SingleSecurityPath<TSecurityContext>(securityPath, required, key);

    public static SecurityPath<TDomainObject> Create<TSecurityContext>(
        Expression<Func<TDomainObject, IEnumerable<TSecurityContext>>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        new ManySecurityPath<TSecurityContext>(securityPath, required, key);

    public static SecurityPath<TDomainObject> CreateNested<TNestedObject>(
        Expression<Func<TDomainObject, IEnumerable<TNestedObject>>> nestedObjectsPath,
        SecurityPath<TNestedObject> nestedSecurityPath,
        bool required = false) =>
        new NestedManySecurityPath<TNestedObject>(nestedObjectsPath, nestedSecurityPath, required);

    #endregion

    public record ConditionPath(Expression<Func<TDomainObject, bool>> FilterExpression) : SecurityPath<TDomainObject>
    {
        private int? hashCode;

        protected ConditionPath(ConditionPath source)
            : base(source)
        {
            this.FilterExpression = source.FilterExpression;
        }

        public override ImmutableArray<Type> UsedSecurityContextTypes => ImmutableArray<Type>.Empty;

        public override SecurityPath<TNewDomainObject> OverrideInput<TNewDomainObject>(
            Expression<Func<TNewDomainObject, TDomainObject>> selector) =>
            new SecurityPath<TNewDomainObject>.ConditionPath(this.FilterExpression.OverrideInput(selector));

        public virtual bool Equals(ConditionPath? other) =>
            ReferenceEquals(this, other)
            || (other is not null
                && this.GetHashCode() == other.GetHashCode()
                && ExpressionComparer.Default.Equals(this.FilterExpression, other.FilterExpression));

        public override int GetHashCode() => this.hashCode ??= ExpressionComparer.Default.GetHashCode(this.FilterExpression);
    }

    public abstract record BinarySecurityPath(SecurityPath<TDomainObject> Left, SecurityPath<TDomainObject> Right)
        : SecurityPath<TDomainObject>
    {
        public override ImmutableArray<Type> UsedSecurityContextTypes { get; } = [.. Left.UsedSecurityContextTypes.Union(Right.UsedSecurityContextTypes)];
    }

    public record OrSecurityPath(SecurityPath<TDomainObject> Left, SecurityPath<TDomainObject> Right) : BinarySecurityPath(Left, Right)
    {
        private int? hashCode;

        protected OrSecurityPath(OrSecurityPath source)
            : base(source)
        {
        }

        public override SecurityPath<TNewDomainObject> OverrideInput<TNewDomainObject>(
            Expression<Func<TNewDomainObject, TDomainObject>> selector) =>
            new SecurityPath<TNewDomainObject>.OrSecurityPath(this.Left.OverrideInput(selector), this.Right.OverrideInput(selector));

        public virtual bool Equals(OrSecurityPath? other) =>
            ReferenceEquals(this, other)
            || (other is not null
                && this.GetHashCode() == other.GetHashCode()
                && EqualityComparer<SecurityPath<TDomainObject>>.Default.Equals(this.Left, other.Left)
                && EqualityComparer<SecurityPath<TDomainObject>>.Default.Equals(this.Right, other.Right));

        public override int GetHashCode() => this.hashCode ??= HashCode.Combine(this.Left, this.Right);
    }

    public record AndSecurityPath(SecurityPath<TDomainObject> Left, SecurityPath<TDomainObject> Right) : BinarySecurityPath(Left, Right)
    {
        private int? hashCode;

        protected AndSecurityPath(AndSecurityPath source)
            : base(source)
        {
        }

        public override SecurityPath<TNewDomainObject> OverrideInput<TNewDomainObject>(
            Expression<Func<TNewDomainObject, TDomainObject>> selector) =>
            new SecurityPath<TNewDomainObject>.AndSecurityPath(
                this.Left.OverrideInput(selector),
                this.Right.OverrideInput(selector));

        public virtual bool Equals(AndSecurityPath? other) =>
            ReferenceEquals(this, other)
            || (other is not null
                && this.GetHashCode() == other.GetHashCode()
                && EqualityComparer<SecurityPath<TDomainObject>>.Default.Equals(this.Left, other.Left)
                && EqualityComparer<SecurityPath<TDomainObject>>.Default.Equals(this.Right, other.Right));

        public override int GetHashCode() => this.hashCode ??= HashCode.Combine(this.Left, this.Right);
    }

    public record SingleSecurityPath<TSecurityContext>(
        Expression<Func<TDomainObject, TSecurityContext?>> Expression,
        bool Required,
        string? Key) : SecurityPath<TDomainObject>, IContextSecurityPath
        where TSecurityContext : ISecurityContext
    {
        private int? hashCode;

        protected SingleSecurityPath(SingleSecurityPath<TSecurityContext> source)
            : base(source)
        {
            this.Expression = source.Expression;
            this.Required = source.Required;
            this.Key = source.Key;
        }

        Type IContextSecurityPath.SecurityContextType { get; } = typeof(TSecurityContext);

        public override ImmutableArray<Type> UsedSecurityContextTypes { get; } = [typeof(TSecurityContext)];

        public override SecurityPath<TNewDomainObject> OverrideInput<TNewDomainObject>(
            Expression<Func<TNewDomainObject, TDomainObject>> selector) =>
            new SecurityPath<TNewDomainObject>.SingleSecurityPath<TSecurityContext>(
                this.Expression.OverrideInput(selector),
                this.Required,
                this.Key);

        public virtual bool Equals(SingleSecurityPath<TSecurityContext>? other) =>
            ReferenceEquals(this, other)
            || (other is not null
                && this.GetHashCode() == other.GetHashCode()
                && this.Required == other.Required
                && this.Key == other.Key
                && ExpressionComparer.Default.Equals(this.Expression, other.Expression));

        public override int GetHashCode() =>
            this.hashCode ??= HashCode.Combine(
                this.Required,
                this.Key,
                ExpressionComparer.Default.GetHashCode(this.Expression));
    }

    public record ManySecurityPath<TSecurityContext>(
        Expression<Func<TDomainObject, IEnumerable<TSecurityContext>>> Expression,
        bool Required,
        string? Key) : SecurityPath<TDomainObject>, IContextSecurityPath
        where TSecurityContext : ISecurityContext
    {
        private int? hashCode;

        protected ManySecurityPath(ManySecurityPath<TSecurityContext> source)
            : base(source)
        {
            this.Expression = source.Expression;
            this.Required = source.Required;
            this.Key = source.Key;
        }

        Type IContextSecurityPath.SecurityContextType { get; } = typeof(TSecurityContext);

        public override ImmutableArray<Type> UsedSecurityContextTypes { get; } = [typeof(TSecurityContext)];

        public override SecurityPath<TNewDomainObject> OverrideInput<TNewDomainObject>(
            Expression<Func<TNewDomainObject, TDomainObject>> selector) =>
            new SecurityPath<TNewDomainObject>.ManySecurityPath<TSecurityContext>(
                this.Expression.OverrideInput(selector),
                this.Required,
                this.Key);

        public virtual bool Equals(ManySecurityPath<TSecurityContext>? other) =>
            ReferenceEquals(this, other)
            || (other is not null
                && this.GetHashCode() == other.GetHashCode()
                && this.Required == other.Required
                && this.Key == other.Key
                && ExpressionComparer.Default.Equals(this.Expression, other.Expression));

        public override int GetHashCode() =>
            this.hashCode ??= HashCode.Combine(
                this.Required,
                this.Key,
                ExpressionComparer.Default.GetHashCode(this.Expression));
    }

    public record NestedManySecurityPath<TNestedObject>(
        Expression<Func<TDomainObject, IEnumerable<TNestedObject>>> NestedExpression,
        SecurityPath<TNestedObject> NestedSecurityPath,
        bool Required) : SecurityPath<TDomainObject>
    {
        private int? hashCode;

        protected NestedManySecurityPath(NestedManySecurityPath<TNestedObject> source)
            : base(source)
        {
            this.NestedExpression = source.NestedExpression;
            this.NestedSecurityPath = source.NestedSecurityPath;
            this.Required = source.Required;
        }

        public override ImmutableArray<Type> UsedSecurityContextTypes { get; } = NestedSecurityPath.UsedSecurityContextTypes;

        public override SecurityPath<TNewDomainObject> OverrideInput<TNewDomainObject>(
            Expression<Func<TNewDomainObject, TDomainObject>> selector) =>
            new SecurityPath<TNewDomainObject>.NestedManySecurityPath<TNestedObject>(
                this.NestedExpression.OverrideInput(selector),
                this.NestedSecurityPath,
                this.Required);

        public virtual bool Equals(NestedManySecurityPath<TNestedObject>? other) =>
            ReferenceEquals(this, other)
            || (other is not null
                && this.GetHashCode() == other.GetHashCode()
                && this.Required == other.Required
                && this.NestedSecurityPath == other.NestedSecurityPath
                && ExpressionComparer.Default.Equals(this.NestedExpression, other.NestedExpression));


        public override int GetHashCode() =>
            this.hashCode ??= HashCode.Combine(
                ExpressionComparer.Default.GetHashCode(this.NestedExpression),
                this.NestedSecurityPath,
                this.Required);
    }
}