using CommonFramework.Maybe;
using CommonFramework.Visitor;

using System.Linq.Expressions;
using System.Reflection;

namespace CommonFramework;

public static class ExpressionExtensions
{
    public static Expression<Func<TArg1, TArg2, TResult>> UnCurrying<TArg1, TArg2, TResult>(this Expression<Func<TArg1, Expression<Func<TArg2, TResult>>>> baseExpr)
    {
        var quoteLambda = (UnaryExpression)baseExpr.Body;
        var innerLambda = (LambdaExpression)quoteLambda.Operand;

        var arg1 = baseExpr.Parameters.Single();
        var arg2 = innerLambda.Parameters.Single();

        return Expression.Lambda<Func<TArg1, TArg2, TResult>>(innerLambda.Body, arg1, arg2);
    }

    extension<TSource, TProperty>(Expression<Func<TSource, TProperty>> path)
	{
		public PropertyAccessors<TSource, TProperty> ToPropertyAccessors() => new (path);

		public Expression<Action<TSource, TProperty>> ToSetLambdaExpression()
		{
			return path.GetProperty().ToSetLambdaExpression<TSource, TProperty>();
        }

        public Func<TSource, TProperty> ToGetFunc()
        {
            return path.GetProperty().GetGetValueFunc<TSource, TProperty>();
        }

        public Action<TSource, TProperty> ToSetAction()
        {
            return path.GetProperty().GetSetValueAction<TSource, TProperty>();
        }

        public Action<TSource, TProperty> ToLazySetAction()
        {
            Lazy<Action<TSource, TProperty>> lazySet = new(path.ToSetAction);

            return (source, value) => lazySet.Value(source, value);
        }

        public Expression<Func<TNextSource, TProperty>> OverrideInput<TNextSource>(Expression<Func<TNextSource, TSource>> expr1)
        {
            return Expression.Lambda<Func<TNextSource, TProperty>>(path.Body.Override(path.Parameters.Single(), expr1.Body), expr1.Parameters);
        }
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
        public Expression<Func<T, bool>> BuildAnd()
        {
            return source.Match(() => _ => true,
                single => single,
                many => many.Aggregate(BuildAnd));
        }

        public Expression<Func<T, bool>> BuildOr()
        {
            return source.Match(() => _ => false,
                single => single,
                many => many.Aggregate(BuildOr));
        }
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
        public Expression<Func<TResult, bool>> BuildAnd<TResult>(Func<TSource, Expression<Func<TResult, bool>>> selector)
        {
            return source.Select(selector).BuildAnd();
        }

        public Expression<Func<TResult, bool>> BuildOr<TResult>(Func<TSource, Expression<Func<TResult, bool>>> selector)
        {
            return source.Select(selector).BuildOr();
        }
    }

    extension<T>(Expression<Func<T, bool>> expr)
    {
        public Expression<Func<T, bool>> Not()
        {
            return
                from v in expr
                select !v;
        }

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

        public Expression<Func<T, bool>> BuildAnd(Expression<Func<T, bool>> expr2)
        {
            return
                from v1 in expr
                from v2 in expr2
                select v1 && v2;
        }

        public Expression<Func<T, bool>> BuildOr(Expression<Func<T, bool>> expr2)
        {
            return
                from v1 in expr
                from v2 in expr2
                select v1 || v2;
        }
    }

    extension<TDelegate>(Expression<TDelegate> expression)
    {
        public Expression<TDelegate> Optimize()
        {
            return expression.UpdateBody(OptimizeBooleanLogicVisitor.Value);
        }

        public Expression<TDelegate> ExpandConst()
        {
            return expression.UpdateBody(ExpandConstVisitor.Value);
        }

        public Expression<TDelegate> UpdateBody(ExpressionVisitor bodyVisitor)
        {
            return expression.Update(bodyVisitor.Visit(expression.Body), expression.Parameters);
        }
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

        public LambdaExpression UpdateBodyBase(ExpressionVisitor bodyVisitor)
        {
            return Expression.Lambda(bodyVisitor.Visit(expression.Body), expression.Parameters);
        }

        public Expression GetBodyWithOverrideParameters(params Expression[] newExpressions)
        {
            var pairs = expression.Parameters.ZipStrong(newExpressions, (parameter, newExpression) => new { Parameter = parameter, NewExpression = newExpression });

            return pairs.Aggregate(expression.Body, (expr, pair) => expr.Override(pair.Parameter, pair.NewExpression));
        }
    }

    /// <param name="baseExpression">expression to get value from</param>
    extension(Expression baseExpression)
    {
        public Expression Override(Expression oldExpr, Expression newExpr)
        {
            return new OverrideExpressionVisitor(oldExpr, newExpr).Visit(baseExpression)!;
        }

        public Expression UpdateBase(IEnumerable<ExpressionVisitor> visitors)
        {
            return visitors.Aggregate(baseExpression, (expr, visitor) => visitor.Visit(expr));
        }

        public Expression UpdateBase(params ExpressionVisitor[] visitors)
        {
            return baseExpression.UpdateBase((IEnumerable<ExpressionVisitor>)visitors);
        }

        public Maybe<TValue?> GetDeepMemberConstValue<TValue>()
        {
            return GetDeepMemberConstValue(baseExpression).Where(v => v is TValue).Select(v => (TValue?)v);
        }

        public Maybe<object?> GetDeepMemberConstValue()
        {
            return baseExpression.GetDeepMemberConstExpression().Select(expr => expr.Value);
        }

        public Maybe<ConstantExpression> GetMemberConstExpression()
        {
            return (baseExpression as ConstantExpression).ToMaybe()

                .Or(() =>

                    from memberExpr in (baseExpression as MemberExpression).ToMaybe()

                    from constExpr in (memberExpr.Expression as ConstantExpression).ToMaybe()

                    from fieldInfo in (memberExpr.Member as FieldInfo).ToMaybe()

                    select Expression.Constant(fieldInfo.GetValue(constExpr.Value), fieldInfo.FieldType));
        }

        /// <summary> Returns constant value from expression
        /// </summary>
        /// <typeparam name="TValue">cast value to specified Type if possible</typeparam>
        /// <returns>constant value of specified Type</returns>
        public Maybe<TValue?> GetMemberConstValue<TValue>()
        {
            return baseExpression.GetMemberConstValue().Where(v => v is TValue).Select(v => (TValue?)v);
        }

        public Maybe<object?> GetMemberConstValue()
        {
            return baseExpression.GetMemberConstExpression().Select(expr => expr.Value);
        }

        public Maybe<ConstantExpression> GetDeepMemberConstExpression()
        {
            var result = baseExpression.GetPureDeepMemberConstExpression();
            if (result == null)
            {
                return Maybe<ConstantExpression>.Nothing;
            }

            return baseExpression is ConstantExpression constExpr ? constExpr.ToMaybe() : Maybe.Maybe.Return(result);
        }

        public ConstantExpression? GetPureDeepMemberConstExpression()
        {
            if (baseExpression is ConstantExpression baseConstExpr)
            {
                return baseConstExpr;
            }

            if (baseExpression is not MemberExpression memberExpr)
            {
                return null;
            }

            var memberChains = memberExpr.GetAllElements(z => z.Expression as MemberExpression).ToList();

            var startExpr = memberChains.Last();

            if (startExpr.Expression is ConstantExpression constExpr)
            {
                var constValue = ValueTuple.Create(startExpr.Member.GetValue(constExpr.Value), startExpr.Member.GetMemberType());
                memberChains.Reverse();
                var finalValue = memberChains
                    .Skip(1) // выше обработали самый первый (var startExpr = memberChains.Last();)
                    .Select(z => z.Member)
                    .Aggregate(
                        constValue,
                        (prevValue, memberInfo) => ValueTuple.Create(memberInfo.GetValue(prevValue.Item1), memberInfo.GetMemberType()));

                if (finalValue.Item1 == null && finalValue.Item2.IsValueType)
                {
                    return null;
                }

                return Expression.Constant(finalValue.Item1, finalValue.Item2);
            }
            else
            {
                return null;
            }
        }

        public Maybe<MemberInfo> GetMember()
        {
            return (baseExpression as UnaryExpression).ToMaybe().Where(unaryExpr => unaryExpr.NodeType == ExpressionType.Convert)
                .SelectMany(unaryExpr => unaryExpr.Operand.GetMember())

                .Or(() => (baseExpression as MethodCallExpression).ToMaybe().Select(callExpr => (MemberInfo)callExpr.Method))

                .Or(() => (baseExpression as MemberExpression).ToMaybe().Select(memberExpr => memberExpr.Member));
        }
    }

    extension(MemberInfo source)
    {
        private object? GetValue(object? arg)
        {
            return source switch
            {
                FieldInfo field => field.GetValue(arg),
                PropertyInfo property => property.GetValue(arg),
                _ => throw new ArgumentOutOfRangeException(nameof(source))
            };
        }

        private Type GetMemberType()
        {
            return source switch
            {
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                _ => throw new ArgumentOutOfRangeException(nameof(source))
            };
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