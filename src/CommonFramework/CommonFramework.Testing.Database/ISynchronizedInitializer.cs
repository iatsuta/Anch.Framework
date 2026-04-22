namespace CommonFramework.Testing.Database;

public interface ISynchronizedInitializer<T>
{
    ValueTask Run(Func<Task> action);
}