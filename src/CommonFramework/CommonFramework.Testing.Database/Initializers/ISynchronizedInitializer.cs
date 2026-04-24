namespace CommonFramework.Testing.Database.Initializers;

public interface ISynchronizedInitializer<T>
{
    Task Run(Func<Task> action);
}