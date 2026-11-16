using System.Collections.Immutable;
using System.Linq.Expressions;

using Anch.Core;
using Anch.Core.ExpressionComparers;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>
/// Security path rule: lambdas that link a domain object to its security contexts.
/// </summary>
/// <typeparam name="TDomainObject">The protected domain type.</typeparam>
/// <remarks>
/// A path tells the security system which security contexts (business unit, location, ...) a domain object belongs to,
/// so that the user's permissions, which are limited by contexts, can be matched against the object.
/// Paths are values and combine with <c>And</c> and <c>Or</c>.
/// </remarks>
/// <example>
/// <code>
/// var path = SecurityPath&lt;Order&gt;.Create(o =&gt; o.BusinessUnit).And(o =&gt; o.Location);
/// </code>
/// </example>
public abstract record SecurityPath<TDomainObject>
{
    /// <summary>The security context types this path refers to.</summary>
    public abstract ImmutableArray<Type> UsedSecurityContextTypes { get; }

    /// <summary>Rebases the path onto another type that can reach the original domain object.</summary>
    /// <param name="selector">Selects the original domain object from the new one.</param>
    /// <typeparam name="TNewDomainObject">The new root type.</typeparam>
    public abstract SecurityPath<TNewDomainObject> OverrideInput<TNewDomainObject>(
        Expression<Func<TNewDomainObject, TDomainObject>> selector);

    /// <summary>A path without security contexts that accepts every object.</summary>
    public static SecurityPath<TDomainObject> Empty { get; } = Condition(_ => true);

    #region Create

    /// <summary>Combines the paths: both must be satisfied.</summary>
    /// <param name="other">The other path.</param>
    public SecurityPath<TDomainObject> And(SecurityPath<TDomainObject> other) => new AndSecurityPath(this, other);

    /// <summary>Adds a plain condition on the object: both the path and the condition must hold.</summary>
    /// <param name="securityFilter">The condition.</param>
    public SecurityPath<TDomainObject> And(Expression<Func<TDomainObject, bool>> securityFilter) => this.And(Condition(securityFilter));

    /// <summary>Adds a single-valued security context: both the path and the context must be satisfied.</summary>
    /// <param name="securityPath">Selects the object's security context.</param>
    /// <param name="required">Whether the object must have a value for this context.</param>
    /// <param name="key">Distinguishes several paths to the same context type.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    public SecurityPath<TDomainObject> And<TSecurityContext>(
        Expression<Func<TDomainObject, TSecurityContext?>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        this.And(Create(securityPath, required, key));

    /// <summary>Adds a multi-valued security context: both the path and the context must be satisfied.</summary>
    /// <param name="securityPath">Selects the object's security contexts.</param>
    /// <param name="required">Whether the object must have a value for this context.</param>
    /// <param name="key">Distinguishes several paths to the same context type.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    public SecurityPath<TDomainObject> And<TSecurityContext>(
        Expression<Func<TDomainObject, IEnumerable<TSecurityContext>>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        this.And(Create(securityPath, required, key));

    /// <summary>Combines the paths: either may be satisfied.</summary>
    /// <param name="other">The other path.</param>
    public SecurityPath<TDomainObject> Or(SecurityPath<TDomainObject> other) => new OrSecurityPath(this, other);

    /// <summary>Adds a plain condition on the object as an alternative.</summary>
    /// <param name="securityFilter">The condition.</param>
    public SecurityPath<TDomainObject> Or(Expression<Func<TDomainObject, bool>> securityFilter) => this.Or(Condition(securityFilter));

    /// <summary>Adds a single-valued security context as an alternative.</summary>
    /// <param name="securityPath">Selects the object's security context.</param>
    /// <param name="required">Whether the object must have a value for this context.</param>
    /// <param name="key">Distinguishes several paths to the same context type.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    public SecurityPath<TDomainObject> Or<TSecurityContext>(
        Expression<Func<TDomainObject, TSecurityContext?>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        this.Or(Create(securityPath, required, key));

    /// <summary>Adds a multi-valued security context as an alternative.</summary>
    /// <param name="securityPath">Selects the object's security contexts.</param>
    /// <param name="required">Whether the object must have a value for this context.</param>
    /// <param name="key">Distinguishes several paths to the same context type.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    public SecurityPath<TDomainObject> Or<TSecurityContext>(
        Expression<Func<TDomainObject, IEnumerable<TSecurityContext>>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        this.Or(Create(securityPath, required, key));

    /// <summary>Creates a path from a plain condition on the object.</summary>
    /// <param name="securityFilter">The condition.</param>
    public static SecurityPath<TDomainObject> Condition(Expression<Func<TDomainObject, bool>> securityFilter) =>
        new ConditionPath(securityFilter);

    /// <summary>Creates a path through a single-valued security context property.</summary>
    /// <param name="securityPath">Selects the object's security context.</param>
    /// <param name="required">Whether the object must have a value for this context.</param>
    /// <param name="key">Distinguishes several paths to the same context type.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    public static SecurityPath<TDomainObject> Create<TSecurityContext>(
        Expression<Func<TDomainObject, TSecurityContext?>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        new SingleSecurityPath<TSecurityContext>(securityPath, required, key);

    /// <summary>Creates a path through a collection of security contexts.</summary>
    /// <param name="securityPath">Selects the object's security contexts.</param>
    /// <param name="required">Whether the object must have a value for this context.</param>
    /// <param name="key">Distinguishes several paths to the same context type.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
    public static SecurityPath<TDomainObject> Create<TSecurityContext>(
        Expression<Func<TDomainObject, IEnumerable<TSecurityContext>>> securityPath,
        bool required = false,
        string? key = null)
        where TSecurityContext : ISecurityContext =>
        new ManySecurityPath<TSecurityContext>(securityPath, required, key);

    /// <summary>Creates a path that delegates to the security path of nested objects (for example the lines of an order).</summary>
    /// <param name="nestedObjectsPath">Selects the nested objects.</param>
    /// <param name="nestedSecurityPath">The path of a nested object.</param>
    /// <param name="required">When <c>true</c>, the object must have nested objects.</param>
    /// <typeparam name="TNestedObject">The type of the nested objects.</typeparam>
    public static SecurityPath<TDomainObject> CreateNested<TNestedObject>(
        Expression<Func<TDomainObject, IEnumerable<TNestedObject>>> nestedObjectsPath,
        SecurityPath<TNestedObject> nestedSecurityPath,
        bool required = false) =>
        new NestedManySecurityPath<TNestedObject>(nestedObjectsPath, nestedSecurityPath, required);

    #endregion

    /// <summary>A path made of a plain condition.</summary>
    /// <param name="FilterExpression">The condition.</param>
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

    /// <summary>A path that combines two paths.</summary>
    /// <param name="Left">The first path.</param>
    /// <param name="Right">The second path.</param>
    public abstract record BinarySecurityPath(SecurityPath<TDomainObject> Left, SecurityPath<TDomainObject> Right)
        : SecurityPath<TDomainObject>
    {
        public override ImmutableArray<Type> UsedSecurityContextTypes { get; } = [.. Left.UsedSecurityContextTypes.Union(Right.UsedSecurityContextTypes)];
    }

    /// <summary>Satisfied when either path is satisfied.</summary>
    /// <param name="Left">The first path.</param>
    /// <param name="Right">The second path.</param>
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

    /// <summary>Satisfied when both paths are satisfied.</summary>
    /// <param name="Left">The first path.</param>
    /// <param name="Right">The second path.</param>
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

    /// <summary>A path to one security context object.</summary>
    /// <param name="Expression">Selects the context.</param>
    /// <param name="Required">Whether the object must have a context.</param>
    /// <param name="Key">Distinguishes several paths to the same context type.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
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
            this.SecurityContextType = source.SecurityContextType;
        }

        public Type SecurityContextType { get; } = typeof(TSecurityContext);

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

    /// <summary>A path to a collection of security context objects.</summary>
    /// <param name="Expression">Selects the contexts.</param>
    /// <param name="Required">Whether the object must have a context.</param>
    /// <param name="Key">Distinguishes several paths to the same context type.</param>
    /// <typeparam name="TSecurityContext">The security context type.</typeparam>
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
            this.SecurityContextType = source.SecurityContextType;
        }

        public Type SecurityContextType { get; } = typeof(TSecurityContext);

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

    /// <summary>A path that applies another path to each nested object.</summary>
    /// <param name="NestedExpression">Selects the nested objects.</param>
    /// <param name="NestedSecurityPath">The path of a nested object.</param>
    /// <param name="Required">Whether the object must have nested objects.</param>
    /// <typeparam name="TNestedObject">The type of the nested objects.</typeparam>
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