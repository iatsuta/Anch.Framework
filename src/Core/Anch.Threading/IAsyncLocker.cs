namespace Anch.Threading;

public interface IAsyncLocker
{
    ValueTask<IDisposable> CreateScope();
}