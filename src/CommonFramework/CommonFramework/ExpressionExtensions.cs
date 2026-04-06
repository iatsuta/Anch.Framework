using System.Collections.Concurrent;

using CommonFramework.Visitor;

using System.Linq.Expressions;
using System.Reflection;

namespace CommonFramework;

public static class ExpressionExtensions
{
    public static Expression<Func<TArg1, TArg2, TResult>> UnCurrying<TArg1, TArg2, TResult>(
        this Expression<Func<TArg1, Expression<Func<TArg2, TResult>>>> baseExpr)
    {
        var quoteLambda = (UnaryExpression)baseExpr.Body;
        var innerLambda = (LambdaExpression)quoteLambda.Operand;

        var arg1 = baseExpr.Parameters.Single();
        var arg2 = innerLambda.Parameters.Single();

        return Expression.Lambda<Func<TArg1, TArg2, TResult>>(innerLambda.Body, arg1, arg2);
    }

    extension<TSource, TProperty>(Expression<Func<TSource, TProperty>> path)
    {
        public PropertyAccessors<TSource, TProperty> ToPropertyAccessors() => new(path);

        public Expression<Action<TSource, TProperty>> ToSetLambdaExpression() => path.GetProperty().ToSetLambdaExpression<TSource, TProperty>();

        public Func<TSource, TProperty> ToGetFunc() => path.GetProperty().GetGetValueFunc<TSource, TProperty>();

        public Action<TSource, TProperty> ToSetAction() => path.GetProperty().GetSetValueAction<TSource, TProperty>();

        public Action<TSource, TProperty> ToLazySetAction()
        {
            Lazy<Action<TSource, TProperty>> lazySet = new(path.ToSetAction);

            return (source, value) => lazySet.Value(source, value);
        }

        public Expression<Func<TNextSource, TProperty>> OverrideInput<TNextSource>(Expression<Func<TNextSource, TSource>> expr1) =>

            Expression.Lambda<Func<TNextSource, TProperty>>(path.Body.Override(path.Parameters.Single(), expr1.Body), expr1.Parameters);
    }

    public static IEnumerable<Expression> GetChildren(this MethodCallExpression expression)
    {
        if (expression.Object != null)
        {
            yield return expression.Object;
        }

        foreach (var arg in expression.Arguments)
        {
            yield return arg;
        }
    }

    extension<T>(IEnumerable<Expression<Func<T, bool>>> source)
    {
        public Expression<Func<T, bool>> BuildAnd() =>

            source.Match(() => _ => true,
                single => single,
                many => many.Aggregate(BuildAnd));

        public Expression<Func<T, bool>> BuildOr() =>

            source.Match(() => _ => false,
                single => single,
                many => many.Aggregate(BuildOr));
    }

    extension<T1, T2>(Expression<Func<T1, T2, bool>> expr1)
    {
        public Expression<Func<T1, T2, bool>> BuildAnd(Expression<Func<T1, T2, bool>> expr2)
        {
            var newExpr2Body = expr2.GetBodyWithOverrideParameters(expr1.Parameters.ToArray<Expression>());

            var newBody = Expression.AndAlso(expr1.Body, newExpr2Body);

            return Expression.Lambda<Func<T1, T2, bool>>(newBody, expr1.Parameters);
        }

        public Expression<Func<T1, T2, bool>> BuildOr(Expression<Func<T1, T2, bool>> expr2)
        {
            var newExpr2Body = expr2.GetBodyWithOverrideParameters(expr1.Parameters.ToArray<Expression>());

            var newBody = Expression.OrElse(expr1.Body, newExpr2Body);

            return Expression.Lambda<Func<T1, T2, bool>>(newBody, expr1.Parameters);
        }
    }

    extension<TSource>(IEnumerable<TSource> source)
    {
        public Expression<Func<TResult, bool>> BuildAnd<TResult>(Func<TSource, Expression<Func<TResult, bool>>> selector) => source.Select(selector).BuildAnd();

        public Expression<Func<TResult, bool>> BuildOr<TResult>(Func<TSource, Expression<Func<TResult, bool>>> selector) => source.Select(selector).BuildOr();
    }

    extension<T>(Expression<Func<T, bool>> expr)
    {
        public Expression<Func<T, bool>> Not() =>

            from v in expr

            select !v;

        public Expression<Func<IEnumerable<T>, bool>> ToEnumerableAny(string? paramName = null)
        {
            var param = Expression.Parameter(typeof(IEnumerable<T>), paramName);

            var anyMethod = new Func<IEnumerable<T>, Func<T, bool>, bool>(Enumerable.Any).Method;

            var callExpr = Expression.Call(null, anyMethod, param, expr);

            return Expression.Lambda<Func<IEnumerable<T>, bool>>(callExpr, param);
        }

        public Expression<Func<IEnumerable<T>, IEnumerable<T>>> ToCollectionFilter(string? paramName = null)
        {
            var param = Expression.Parameter(typeof(IEnumerable<T>), paramName);

            var whereMethod = new Func<IEnumerable<T>, Func<T, bool>, IEnumerable<T>>(Enumerable.Where).Method;

            return Expression.Lambda<Func<IEnumerable<T>, IEnumerable<T>>>(Expression.Call(null, whereMethod, param, expr), param);
        }

        public Expression<Func<T, bool>> BuildAnd(Expression<Func<T, bool>> expr2) =>

            from v1 in expr
            from v2 in expr2
            select v1 && v2;

        public Expression<Func<T, bool>> BuildOr(Expression<Func<T, bool>> expr2) =>

            from v1 in expr
            from v2 in expr2
            select v1 || v2;
    }

    extension<TDelegate>(Expression<TDelegate> expression)
    {
        public Expression<TDelegate> Optimize() => expression.UpdateBody(OptimizeBooleanLogicVisitor.Value);

        public Expression<TDelegate> ExpandConst() => expression.UpdateBody(ExpandConstVisitor.Value);

        public Expression<TDelegate> UpdateBody(ExpressionVisitor bodyVisitor) => expression.Update(bodyVisitor.Visit(expression.Body), expression.Parameters);
    }

    extension(LambdaExpression expression)
    {
        public PropertyInfo GetProperty()
        {
            var request =

                from member in expression.Body.GetMember()

                from property in (member as PropertyInfo).ToMaybe()

                select property;

            return request.GetValue(() => new ArgumentException("not property expression", nameof(expression)));
        }

        public LambdaExpression UpdateBodyBase(ExpressionVisitor bodyVisitor) => Expression.Lambda(bodyVisitor.Visit(expression.Body), expression.Parameters);

        public Expression GetBodyWithOverrideParameters(params Expression[] newExpressions)
        {
            var pairs = expression.Parameters.ZipStrong(newExpressions,
                (parameter, newExpression) => new { Parameter = parameter, NewExpression = newExpression });

            return pairs.Aggregate(expression.Body, (expr, pair) => expr.Override(pair.Parameter, pair.NewExpression));
        }
    }

    extension(Expression baseExpression)
    {
        public Expression ExtractBoxingValue() => baseExpression.GetConvertOperand().GetValueOrDefault(baseExpression);

        public Maybe<Expression> GetConvertOperand() =>

            from unaryExpression in (baseExpression as UnaryExpression).ToMaybe()

            where unaryExpression.NodeType == ExpressionType.Convert

            select unaryExpression.Operand;

        public Expression Override(Expression oldExpr, Expression newExpr) => new OverrideExpressionVisitor(oldExpr, newExpr).Visit(baseExpression)!;

        public Expression UpdateBase(IEnumerable<ExpressionVisitor> visitors) => visitors.Aggregate(baseExpression, (expr, visitor) => visitor.Visit(expr));

        public Expression UpdateBase(params ExpressionVisitor[] visitors) => baseExpression.UpdateBase((IEnumerable<ExpressionVisitor>)visitors);

        public Maybe<TValue> GetConstantValue<TValue>()
        {
            return

                from rawValue in baseExpression.GetConstantValue()

                where rawValue is TValue

                select (TValue)rawValue;
        }

        public Maybe<object?> GetConstantValue() => baseExpression.GetConstantExpression().Select(expr => expr.Value);

        public Maybe<ConstantExpression> GetConstantExpression() => baseExpression.TryGetConstantExpression().ToMaybe();

        public ConstantExpression? TryGetConstantExpression()
        {
            return baseExpression switch
            {
                ConstantExpression constantExpression => constantExpression,

                MemberExpression { Expression: null, Member: { IsStatic: true } member }

                    when member.TryExtractConstantExpression(null) is { } result =>

                    result,

                MemberExpression { Expression: {} baseExpr, Member : { IsStatic: false } member }

                    when baseExpr.TryGetConstantExpression() is { Value: { } baseValue }

                         && member.TryExtractConstantExpression(baseValue) is { } result

                    => result,

                _ => null
            };
        }

        public Maybe<MemberInfo> GetMember()
        {
            return (baseExpression as UnaryExpression).ToMaybe().Where(unaryExpr => unaryExpr.NodeType == ExpressionType.Convert)
                .SelectMany(unaryExpr => unaryExpr.Operand.GetMember())

                .Or(() => (baseExpression as MethodCallExpression).ToMaybe().Select(callExpr => (MemberInfo)callExpr.Method))

                .Or(() => (baseExpression as MemberExpression).ToMaybe().Select(memberExpr => memberExpr.Member));
        }
    }

    private static readonly ConcurrentDictionary<MemberInfo, bool?> IsStaticCache = [];

    extension(MemberInfo memberInfo)
    {
        private ConstantExpression? TryExtractConstantExpression(object? arg) =>

            memberInfo switch
            {
                FieldInfo field => Expression.Constant(field.GetValue(arg)),

                PropertyInfo property => Expression.Constant(property.GetValue(arg)),

                _ => null
            };

        private bool? IsStatic
        {
            get
            {
                return IsStaticCache.GetOrAdd(memberInfo, _ => memberInfo switch
                {
                    FieldInfo { IsStatic: var isStatic } => isStatic,

                    PropertyInfo property
                        when property.GetGetMethod(true) is { IsStatic: var isStatic } => isStatic,

                    _ => null
                });
            }
        }
    }

    internal static Func<object, object, object>? GetBinaryMethod(this ExpressionType type)
    {
        switch (type)
        {
            case ExpressionType.Equal:
                return (v1, v2) => Equals(v1, v2);

            case ExpressionType.NotEqual:
                return (v1, v2) => !Equals(v1, v2);

            case ExpressionType.OrElse:
                return (v1, v2) => ((bool)v1) || ((bool)v2);

            case ExpressionType.AndAlso:
                return (v1, v2) => ((bool)v1) && ((bool)v2);

            default:
                return null;
        }
    }
}