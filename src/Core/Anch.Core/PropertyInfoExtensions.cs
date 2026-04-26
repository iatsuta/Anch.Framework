using System.Linq.Expressions;
using System.Reflection;

using Anch.Core.DictionaryCache;

namespace Anch.Core;

public static class PropertyInfoExtensions
{
    extension(PropertyInfo property)
    {
        public Expression<Func<TSource, TProperty>> ToGetLambdaExpression<TSource, TProperty>()
        {
            return PropertyLambdaCache<TSource, TProperty>.GetLambdaCache[property];
        }

        public Expression<Action<TSource, TProperty>> ToSetLambdaExpression<TSource, TProperty>()
        {
            return PropertyLambdaCache<TSource, TProperty>.SetLambdaCache[property];
        }

        public LambdaExpression ToGetLambdaExpression(Type? sourceType = null)
        {
            return PropertyLambdaCache.GetLambdaCache!.GetValue(property, sourceType ?? property.ReflectedType);
        }

        public LambdaExpression ToSetLambdaExpression(Type? sourceType = null)
        {
            return PropertyLambdaCache.SetLambdaCache!.GetValue(property, sourceType ?? property.ReflectedType);
        }

        public Func<TSource, TProperty> GetGetValueFunc<TSource, TProperty>()
        {
            return PropertyLambdaCache<TSource, TProperty>.GetFuncCache[property];
        }

        public Action<TSource, TProperty> GetSetValueAction<TSource, TProperty>()
        {
            return PropertyLambdaCache<TSource, TProperty>.SetActionCache[property];
        }
    }

    private static class PropertyLambdaCache
    {
        public static readonly IDictionaryCache<Tuple<PropertyInfo, Type>, LambdaExpression> GetLambdaCache =
            new DictionaryCache<Tuple<PropertyInfo, Type>, LambdaExpression>(tuple =>
            {
                var property = tuple.Item1;
                var propertySource = tuple.Item2;

                var parameter = Expression.Parameter(propertySource);

                return Expression.Lambda(Expression.Property(parameter, property), parameter);
            }).WithLock();

        public static readonly IDictionaryCache<Tuple<PropertyInfo, Type>, LambdaExpression> SetLambdaCache =
            new DictionaryCache<Tuple<PropertyInfo, Type>, LambdaExpression>(tuple =>
            {
                var property = tuple.Item1;
                var propertySource = tuple.Item2;

                var sourceParameter = Expression.Parameter(propertySource);
                var valueParameter = Expression.Parameter(property.PropertyType);

                return Expression.Lambda(
                    Expression.Call(sourceParameter,
                        property.GetSetMethod()
                        ?? property.GetSetMethod(true)
                        ?? throw new InvalidOperationException(
                            $"Setter method for property '{property.Name}' on type '{propertySource}' was not found."), valueParameter),
                    sourceParameter,
                    valueParameter);
            }).WithLock();
    }

    private static class PropertyLambdaCache<TSource, TProperty>
    {
        public static readonly IDictionaryCache<PropertyInfo, Expression<Func<TSource, TProperty>>> GetLambdaCache =
            new DictionaryCache<PropertyInfo, Expression<Func<TSource, TProperty>>>(property =>
                (Expression<Func<TSource, TProperty>>)property.ToGetLambdaExpression(typeof(TSource))).WithLock();

        public static readonly IDictionaryCache<PropertyInfo, Func<TSource, TProperty>> GetFuncCache = new DictionaryCache<PropertyInfo, Func<TSource, TProperty>>(property =>
            GetLambdaCache[property].Compile()).WithLock();


        public static readonly IDictionaryCache<PropertyInfo, Expression<Action<TSource, TProperty>>> SetLambdaCache =
            new DictionaryCache<PropertyInfo, Expression<Action<TSource, TProperty>>>(property =>
                (Expression<Action<TSource, TProperty>>)property.ToSetLambdaExpression(typeof(TSource))).WithLock();

        public static readonly IDictionaryCache<PropertyInfo, Action<TSource, TProperty>> SetActionCache = new DictionaryCache<PropertyInfo, Action<TSource, TProperty>>(property =>
            SetLambdaCache[property].Compile()).WithLock();
    }
}