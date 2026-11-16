using System.Runtime.Serialization;

using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

/// <summary>An enumeration constant, kept by its name until it is bound to a type.</summary>
/// <param name="Value">The name of the enumeration member.</param>
[DataContract]
public record EnumConstantExpression(string Value) : ConstantExpression<string>(Value)
{
    /// <summary>Creates the constant from an enumeration value.</summary>
    /// <param name="value">The value.</param>
    public EnumConstantExpression(Enum value)
        : this(value.ToString())
    {
    }

    public override string ToString() => $"\"{this.Value}\"";
}
