namespace Anch.Core;

public interface IFactory<out T>
{
    T Create();
}