using DataModel.Definitions.Enums;

namespace DataModel.Responses.Map;

public class SocialLinkResponse
{
    public SocialLinkTypes SocialLinkType { get; set; }
    
    public string Url { get; set; }
}