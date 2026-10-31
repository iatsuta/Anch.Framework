namespace Anch.OData.Domain.QueryLanguage.Constant.Base;

/// <summary>A constant value in a query.</summary>
public abstract record ConstantExpression : Expression
{
    /// <summary>Returns the value as an object.</summary>
    public abstract object? GetRawValue();

    public override string ToString() => this.GetRawValue()?.ToString()!;
}

/// <summary>A constant of a particular type.</summary>
/// <param name="Value">The value.</param>
/// <typeparam name="TValue">The type of the value.</typeparam>
public abstract record ConstantExpression<TValue>(TValue Value) : ConstantExpression
{
    /// <inheritdoc/>
    public override object? GetRawValue() => this.Value;
}
