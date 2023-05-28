using System;
using System.Collections.Generic;
using DataModel.Responses.Art;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Media;
using Newtonsoft.Json;

namespace DataModel.Responses.Map;

public class EventItemResponse
{
    public string Name { get; set; }

    public string Description { get; set; }

    [JsonProperty("link")]
    public string Site { get; set; }

    public DateTime EventDate { get; set; }

    public string TicketLink { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Phone { get; set; }
    
    public CmsResponse<IEnumerable<CmsItemResponse<StreamResponse>>> Streams { get; set; }

    public CmsResponse<CmsItemResponse<LocationResponse>> Place { get; set; }

    public CmsResponse<CmsItemResponse<CommonMediaItem>> Cover { get; set; }
}
