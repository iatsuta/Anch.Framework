using System.Linq.Expressions;

namespace Anch.Core;

public static class ExpressionHelper
{
    public static MemberExpression PropertyOrFieldAuto(Expression expr, string memberName)
    {
        if (expr == null) throw new ArgumentNullException(nameof(expr));
        if (memberName == null) throw new ArgumentNullException(nameof(memberName));

        if (expr.Type.IsInterface)
        {
            var property = expr.Type.GetAllInterfaceProperties().FirstOrDefault(prop => prop.Name == memberName);

            if (property != null)
            {
                return Expression.Property(expr, property);
            }
        }

        return Expression.PropertyOrField(expr, memberName);
    }

    public static Expression<Func<T, TResult>> Create<T, TResult>(Expression<Func<T, TResult>> func) => func;

    public static Expression<Func<T1, T2, TResult>> Create<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> func) => func;

    public static Expression<Func<T, T>> GetIdentity<T>() => Cache<T>.IdentityExpr;

    public static Expression<Func<T, T, bool>> GetEquality<T>() => Cache<T>.EqualityExpr;

    public static Expression<Func<T, bool>> GetEqualityWithExpr<T>(T value)
	{
		var p1 = Expression.Parameter(typeof(T));

		return Expression.Lambda<Func<T, bool>>(Expression.Equal(p1, Expression.Constant(value)), p1);
	}

	private static class Cache<T>
	{
		public static readonly Expression<Func<T, T>> IdentityExpr = x => x;

		public static readonly Expression<Func<T, T, bool>> EqualityExpr = GetEqualityExpr();

		private static Expression<Func<T, T, bool>> GetEqualityExpr()
		{
			var p1 = Expression.Parameter(typeof(T));
			var p2 = Expression.Parameter(typeof(T));

			return Expression.Lambda<Func<T, T, bool>>(Expression.Equal(p1, p2), p1, p2);
		}
	}
}