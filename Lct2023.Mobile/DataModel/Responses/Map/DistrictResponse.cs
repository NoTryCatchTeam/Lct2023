using DataModel.Definitions.Enums;
using Newtonsoft.Json;

namespace DataModel.Responses.Map;

public class DistrictResponse
{
    [JsonProperty("area")]
    public AreaType AreaType { get; set; }
    
    public string District { get; set; }
}