using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Anch.Testing.Xunit.Engine;

public static class PrivateMemberAccessor
{
    public static Func<TObject, TField> GetInstanceField<TObject, TField>(string fieldName) => InstanceFieldHelper<TObject, TField>.Get(fieldName);

    public static Func<TObject, TDelegate> GetInstanceMethod<TObject, TDelegate>(string methodName) where TDelegate : Delegate => InstanceMemberHelper<TObject, TDelegate>.Get(methodName);

    public static Func<TField> GetStaticField<TField>(Type objectType, string fieldName) =>
        StaticFieldHelper<TField>.Get(objectType, fieldName);

    public static TDelegate GetStaticMethod<TDelegate>(Type objectType, string methodName)
        where TDelegate : Delegate =>
        StaticMethodHelper<TDelegate>.Get(objectType, methodName);

    private static class InstanceFieldHelper<TObject, TField>
    {
        private static readonly ConcurrentDictionary<string, Func<TObject, TField>> Cache = new();

        public static Func<TObject, TField> Get(string fieldName)
        {
            return Cache.GetOrAdd(fieldName, _ =>
            {
                var field = typeof(TObject).GetField(
                                fieldName,
                                BindingFlags.Instance | BindingFlags.NonPublic)
                            ?? throw new InvalidOperationException(
                                $"Private field '{fieldName}' not found in {typeof(TObject)}");

                var instance = Expression.Parameter(typeof(TObject), "instance");
                var access = Expression.Field(instance, field);

                return Expression.Lambda<Func<TObject, TField>>(access, instance).Compile();
            });
        }
    }


    private static class InstanceMemberHelper<TObject, TDelegate>
        where TDelegate : Delegate
    {
        private static readonly ConcurrentDictionary<string, Func<TObject, TDelegate>> Cache = new();

        public static Func<TObject, TDelegate> Get(string methodName)
        {
            return Cache.GetOrAdd(methodName, _ =>
            {
                var method = typeof(TObject).GetMethod(
                                 methodName,
                                 BindingFlags.Instance | BindingFlags.NonPublic)
                             ?? throw new InvalidOperationException(
                                 $"Private method '{methodName}' not found in {typeof(TObject)}");

                return (TDelegate)method.CreateDelegate(typeof(TDelegate));
            });
        }
    }

    private static class StaticFieldHelper<TField>
    {
        private static readonly ConcurrentDictionary<(Type, string), Func<TField>> Cache = new();

        public static Func<TField> Get(Type objectType, string fieldName)
        {
            return Cache.GetOrAdd((objectType, fieldName), _ =>
            {
                var field = objectType.GetField(
                                fieldName,
                                BindingFlags.Static | BindingFlags.NonPublic)
                            ?? throw new InvalidOperationException(
                                $"Private static field '{fieldName}' not found in {objectType}");

                var access = Expression.Field(null, field);

                return Expression.Lambda<Func<TField>>(access).Compile();
            });
        }
    }


    private static class StaticMethodHelper<TDelegate>
        where TDelegate : Delegate
    {
        private static readonly ConcurrentDictionary<(Type, string), TDelegate> Cache = new();

        public static TDelegate Get(Type objectType, string methodName)
        {
            return Cache.GetOrAdd((objectType, methodName), _ =>
            {
                var method = objectType.GetMethod(
                                 methodName,
                                 BindingFlags.Static | BindingFlags.NonPublic)
                             ?? throw new InvalidOperationException(
                                 $"Private static method '{methodName}' not found in {objectType}");

                return (TDelegate)method.CreateDelegate(typeof(TDelegate));
            });
        }
    }
}