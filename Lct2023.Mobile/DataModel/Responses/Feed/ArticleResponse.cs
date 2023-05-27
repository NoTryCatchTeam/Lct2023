using System;
using System.Collections.Generic;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Media;
using Newtonsoft.Json;

namespace DataModel.Responses.Feed;

public class ArticleResponse
{
    public string Title { get; set; }
    
    [JsonProperty("text")]
    public string Description { get; set; }
    
    public DateTime PublishedAt { get; set; }
    
    public string Link { get; set; }
    
    public CmsResponse<CmsItemResponse<RubricResponse>> Rubric { get; set; }
    
    [JsonProperty("art_categories")]
    public CmsResponse<IEnumerable<CmsItemResponse<ArtCategoryResponse>>> ArtCategory { get; set; }
    
    public CmsResponse<CmsItemResponse<CommonMediaItem>> Cover { get; set; }
}