using DataModel.Responses.BaseCms;
using Newtonsoft.Json;

namespace DataModel.Responses.Quizzes;

public class VideoToAudioQuizItemResponse
{
    public string Question { get; set; }

    public CmsResponse<CmsItemResponse<VideoToAudioQuizVideo>> Video { get; set; }

    [JsonProperty("a")]
    public CmsResponse<CmsItemResponse<VideoToAudioQuizAnswer>> AnswerA { get; set; }

    [JsonProperty("b")]
    public CmsResponse<CmsItemResponse<VideoToAudioQuizAnswer>> AnswerB { get; set; }

    [JsonProperty("c")]
    public CmsResponse<CmsItemResponse<VideoToAudioQuizAnswer>> AnswerC { get; set; }

    [JsonProperty("answer")]
    public string CorrectAnswerTag { get; set; }
}