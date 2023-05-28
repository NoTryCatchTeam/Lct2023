using DataModel.Responses.BaseCms;
using Newtonsoft.Json;

namespace DataModel.Responses.Map;

public class LocationResponse
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }

    [JsonProperty("lon")]
    public double Longitude { get; set; }
    
    public string Address { get; set; }
    
    public CmsResponse<CmsItemResponse<DistrictResponse>> District { get; set; }
}