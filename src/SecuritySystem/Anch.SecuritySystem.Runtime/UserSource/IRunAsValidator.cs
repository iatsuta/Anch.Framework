using Anch.SecuritySystem.Validation;

namespace Anch.SecuritySystem.UserSource;

/// <summary>Validates a RunAs request.</summary>
public interface IRunAsValidator : ISecurityValidator<UserCredential>;