using DataModel.Responses.BaseCms;
using Newtonsoft.Json;

namespace DataModel.Responses.Art;

public class StreamResponse
{
    public string Name { get; set; }
    
    [JsonProperty("art_category")]
    public CmsResponse<CmsItemResponse<ArtCategoryResponse>> ArtCategory { get; set; }
}