using DataModel.Responses.BaseCms;
using DataModel.Responses.Media;
using Newtonsoft.Json;

namespace DataModel.Responses.Quizzes;

public class VideoToAudioQuizItemResponse
{
    public string Question { get; set; }

    public CmsResponse<CmsItemResponse<CommonMediaItem>> Video { get; set; }

    [JsonProperty("a")]
    public CmsResponse<CmsItemResponse<CommonMediaItem>> AnswerA { get; set; }

    [JsonProperty("b")]
    public CmsResponse<CmsItemResponse<CommonMediaItem>> AnswerB { get; set; }

    [JsonProperty("c")]
    public CmsResponse<CmsItemResponse<CommonMediaItem>> AnswerC { get; set; }

    [JsonProperty("answer")]
    public string CorrectAnswerTag { get; set; }
}
