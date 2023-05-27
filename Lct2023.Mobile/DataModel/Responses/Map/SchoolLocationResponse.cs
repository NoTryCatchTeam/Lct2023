using System.Collections.Generic;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using Newtonsoft.Json;

namespace DataModel.Responses.Map;

public class SchoolLocationResponse
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }

    [JsonProperty("lon")]
    public double Longitude { get; set; }

    public string Name { get; set; }
    
    public string Email { get; set; }
    
    public string Phone { get; set; }
    
    public string Address { get; set; }

    public bool IsSpecial { get; set; }
    
    public CmsResponse<CmsItemResponse<DistrictResponse>> District { get; set; }
    
    public CmsResponse<IEnumerable<CmsItemResponse<StreamResponse>>> Streams { get; set; }
}
