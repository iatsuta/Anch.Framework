using System.Runtime.Serialization;
using Anch.OData.Domain.QueryLanguage.Constant.Base;

namespace Anch.OData.Domain.QueryLanguage.Constant;

[DataContract]
public record EnumConstantExpression(string Value) : ConstantExpression<string>(Value)
{
    public EnumConstantExpression(Enum value)
        : this(value.ToString())
    {
    }

    public override string ToString() => $"\"{this.Value}\"";
}
