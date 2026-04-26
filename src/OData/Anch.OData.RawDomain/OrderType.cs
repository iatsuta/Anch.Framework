using System.Runtime.Serialization;

namespace Anch.OData.Domain;

[DataContract]
public enum OrderType
{
    [EnumMember]
    Asc,

    [EnumMember]
    Desc
}
