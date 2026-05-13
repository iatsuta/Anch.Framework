namespace Anch.SecuritySystem.Validation;

public interface ISecurityValidator
{
    public const string ElementKey = "Element";
}

public interface ISecurityValidator<in T> : ISecurityValidator
{
    ValueTask ValidateAsync(T value, CancellationToken cancellationToken);
}