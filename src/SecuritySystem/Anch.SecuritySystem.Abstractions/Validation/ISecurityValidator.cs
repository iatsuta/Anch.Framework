namespace Anch.SecuritySystem.Validation;

public interface ISecurityValidator
{
    public const string ElementKey = "Element";
}

public interface ISecurityValidator<in T> : ISecurityValidator
{
	Task ValidateAsync(T value, CancellationToken cancellationToken);
}