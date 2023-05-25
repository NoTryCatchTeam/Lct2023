using DataModel.Definitions.Enums;
using Newtonsoft.Json;

namespace DataModel.Responses.Map;

public class DistrictResponse
{
    [JsonProperty("area")]
    public DistrictType DistrictType { get; set; }
}