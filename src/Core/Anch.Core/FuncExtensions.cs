namespace Anch.Core;

public static class FuncExtensions
{
    extension<TResult>(Func<TResult> func)
    {
	    public Func<TResult> WithCache()
	    {
		    var lazyValue = func.ToLazy();

		    return () => lazyValue.Value;
	    }

	    public Lazy<TResult> ToLazy()
	    {
		    return new Lazy<TResult>(func);
	    }
    }

    public static Func<TArg, TNextResult> Composite<TArg, TResult, TNextResult>(this Func<TArg, TResult> func, Func<TResult, TNextResult> nextFunc)
    {
	    return arg => nextFunc(func(arg));
    }
}