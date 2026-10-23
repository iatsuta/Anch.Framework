namespace Anch.Core;

public interface IFactory<out TResult>
{
    TResult Create();
}

public interface IFactory<in TArg, out TResult>
{
    TResult Create(TArg arg);
}