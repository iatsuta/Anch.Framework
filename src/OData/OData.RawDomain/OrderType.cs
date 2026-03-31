using System.Runtime.Serialization;

namespace OData.Domain;

[DataContract]
public enum OrderType
{
    [EnumMember]
    Asc,

    [EnumMember]
    Desc
}
