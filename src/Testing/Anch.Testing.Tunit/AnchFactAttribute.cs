namespace Anch.Testing.TUnit;

public sealed class AnchTUnitAttribute<TEnvironment>() : ClassConstructorAttribute(typeof(AnchTUnitClassConstructor<TEnvironment>))
    where TEnvironment : ITestEnvironment, new();