using Newtonsoft.Json;

namespace DataModel.Responses.Media;

public class CommonMediaItem
{
    public string Name { get; set; }

    [JsonProperty("ext")]
    public string Extension { get; set; }

    [JsonProperty("mime")]
    public string Mime { get; set; }

    public string Url { get; set; }
}
