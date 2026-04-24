namespace CommonFramework.Threading;

public interface IAsyncLocker
{
    ValueTask<IDisposable> CreateScope();
}