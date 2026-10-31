namespace Anch.Core;

/// <summary>Creates instances of <typeparamref name="TResult"/>.</summary>
/// <typeparam name="TResult">The type of the created objects.</typeparam>
public interface IFactory<out TResult>
{
    /// <summary>Creates an instance.</summary>
    TResult Create();
}

/// <summary>Creates instances of <typeparamref name="TResult"/> from an argument.</summary>
/// <typeparam name="TArg">The type of the argument.</typeparam>
/// <typeparam name="TResult">The type of the created objects.</typeparam>
public interface IFactory<in TArg, out TResult>
{
    /// <summary>Creates an instance.</summary>
    /// <param name="arg">The argument.</param>
    TResult Create(TArg arg);
}