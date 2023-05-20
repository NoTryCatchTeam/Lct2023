using Newtonsoft.Json;

namespace DataModel.Responses.BaseCms
{
    public class CmsItemResponse<TItem>
    {
        public int Id { get; set; }
        
        [JsonProperty("attributes")]
        public TItem Item { get; set; }
    }
}