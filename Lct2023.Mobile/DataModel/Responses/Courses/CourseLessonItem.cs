using DataModel.Responses.BaseCms;
using DataModel.Responses.Media;
using Newtonsoft.Json;

namespace DataModel.Responses.Courses;

public class CourseLessonItem
{
    [JsonProperty("lessonChapter")]
    public int Chapter { get; set; }

    [JsonProperty("lessonNumber")]
    public int Number { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string AdditionalMaterial { get; set; }

    public string Homework { get; set; }

    [JsonProperty("content")]
    public CmsResponse<CmsItemResponse<CommonMediaItem>> Attachment { get; set; }
}