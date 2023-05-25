using System.Runtime.Serialization;

namespace DataModel.Definitions.Enums;

public enum DistrictType
{
    Default,
    
    [EnumMember(Value = "ЦАО")]
    Cao,
    
    [EnumMember(Value = "САО")]
    Sao,
    
    [EnumMember(Value = "СВАО")]
    Svao,
    
    [EnumMember(Value = "ВАО")]
    Vao,
    
    [EnumMember(Value = "ЮВАО")]
    Uvao,
    
    [EnumMember(Value = "ЮАО")]
    Uao,
    
    [EnumMember(Value = "ЮЗАО")]
    Uzao,
    
    [EnumMember(Value = "ЗАО")]
    Zao,
    
    [EnumMember(Value = "СЗАО")]
    Szao,
    
    [EnumMember(Value = "ЗелАО")]
    Zelao,
    
    [EnumMember(Value = "ТинАО")]
    Tinao,
}