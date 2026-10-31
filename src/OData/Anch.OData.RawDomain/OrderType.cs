using System.Runtime.Serialization;

namespace Anch.OData.Domain;

/// <summary>The sort direction of an <c>$orderby</c> clause.</summary>
[DataContract]
public enum OrderType
{
    /// <summary>Ascending.</summary>
    [EnumMember]
    Asc,

    /// <summary>Descending.</summary>
    [EnumMember]
    Desc
}
