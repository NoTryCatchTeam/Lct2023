using System.Runtime.Serialization;

namespace DataModel.Definitions.Enums;

public enum SortingType
{
    [EnumMember(Value = "asc")]
    Asc,
    
    [EnumMember(Value = "desc")]
    Desc
}