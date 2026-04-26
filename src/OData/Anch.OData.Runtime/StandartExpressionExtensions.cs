using System.Reflection;
using Anch.Core;
using SExpressions = System.Linq.Expressions;

namespace Anch.OData;

public static class StandardExpressionExtensions
{
    internal static SExpressions.Expression TryNormalize(this SExpressions.Expression baseExpression, Type leftType, Type rightType)
    {
        var tryNullableType = new[] { leftType, rightType }.FirstOrDefault(type => type.IsNullable()) ?? TryGetNullableFromNull(leftType, rightType);

        var tryEnumType = new[] { leftType, rightType }.FirstOrDefault(type => type.IsEnum);

        var request = from nullableType in tryNullableType.ToMaybe()

            select LiftToNullable(baseExpression, nullableType);

        return request.Or(() => from enumType in tryEnumType.ToMaybe()

                select baseExpression.TryConvertToEnumExpression(enumType))


            .Or(() => from superType in leftType.GetSuperSet(rightType, false).ToMaybe()

                where baseExpression.Type != superType

                select UpToSuperType(baseExpression, superType))

            .GetValueOrDefault(baseExpression);
    }

    private static Type? TryGetNullableFromNull(Type leftType, Type rightType)
    {
        var arr = new[] { leftType, rightType };

        return arr.Contains(typeof(object)) ? arr.Where(t => t.IsValueType).Select(t => typeof(Nullable<>).MakeGenericType(t)).SingleOrDefault() : null;
    }

    private static SExpressions.Expression UpToSuperType(SExpressions.Expression baseExpression, Type superType)
    {
        return (from constValue in baseExpression.GetConstantValue()

                let converterValue = Convert.ChangeType(constValue, superType, null)

                select SExpressions.Expression.Constant(converterValue))

            .GetValueOrDefault(() => (SExpressions.Expression)SExpressions.Expression.Convert(baseExpression, superType));
    }


    public static SExpressions.Expression TryConvertToEnumExpression(this SExpressions.Expression baseExpression, Type enumType)
    {
        var request = from value in baseExpression.GetConstantValue()

            where value != null && enumType.IsEnum && value.GetType() != enumType

            from enumValue in TryConvertToEnum(value!, enumType)

            select SExpressions.Expression.Constant(enumValue);


        return request.GetValueOrDefault(baseExpression);

    }

    private static Maybe<object> TryConvertToEnum(object value, Type enumType)
    {
        return (from strValue in (value as string).ToMaybe()

                select Enum.Parse(enumType, strValue, true))

            .Or(() => from underType in Convert.ChangeType(value, Enum.GetUnderlyingType(enumType), null).ToMaybe()

                select Enum.ToObject(enumType, underType));
    }

    private static SExpressions.Expression LiftToNullable(SExpressions.Expression expression, Type expectedNullableType)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        if (expression.Type.IsNullable())
        {
            return expression;
        }

        if (expression.Type.IsValueType && typeof(Nullable<>).MakeGenericType(expression.Type) == expectedNullableType)
        {
            return SExpressions.Expression.Convert(expression, expectedNullableType);
        }

        return expression.GetConstantValue().Select(value =>
        {
            if (value == null)
            {
                return SExpressions.Expression.Constant(null, expectedNullableType);
            }
            else
            {
                var expectedNullableElementType = expectedNullableType.GetNullableElementType()!;

                if (value.GetType() != expectedNullableElementType && expectedNullableElementType.IsEnum)
                {
                    return LiftToNullable(expression.TryConvertToEnumExpression(expectedNullableElementType), expectedNullableType);
                }
                else
                {
                    var liftedValue = CreateNullableConstantMethod.MakeGenericMethod(expression.Type, expectedNullableElementType).Invoke(null,
                        [value]);

                    return SExpressions.Expression.Constant(liftedValue, expectedNullableType);
                }
            }
        }).GetValue(() => new Exception("fail"));
    }


    private static readonly MethodInfo CreateNullableConstantMethod = new Func<int, int?>(ToNullableValue<int, int>).Method.GetGenericMethodDefinition();

    internal static TExpectedValue? ToNullableValue<TConstValue, TExpectedValue>(TConstValue constValue)
        where TConstValue : struct
        where TExpectedValue : struct =>
        (TExpectedValue)Convert.ChangeType(constValue, typeof(TExpectedValue), null);
}