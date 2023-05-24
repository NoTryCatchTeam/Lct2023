using Newtonsoft.Json;

namespace DataModel.Responses.Map;

public class SchoolLocationResponse
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }

    [JsonProperty("lon")]
    public double Longitude { get; set; }

    public string Name { get; set; }
}
