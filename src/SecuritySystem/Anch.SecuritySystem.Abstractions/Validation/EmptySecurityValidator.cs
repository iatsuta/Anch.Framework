namespace Anch.SecuritySystem.Validation;

public class EmptySecurityValidator<T> : ISecurityValidator<T>
{
    public ValueTask ValidateAsync(T value, CancellationToken ct) => ValueTask.CompletedTask;
}