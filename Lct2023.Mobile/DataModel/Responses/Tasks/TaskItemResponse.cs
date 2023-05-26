using System.Collections.Generic;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Quizzes;
using Newtonsoft.Json;

namespace DataModel.Responses.Tasks;

public class TaskItemResponse
{
    public CmsResponse<IEnumerable<CmsItemResponse<QuizItemResponse>>> Quizzes { get; set; }
    
    [JsonProperty("audio_quizzes")]
    public CmsResponse<IEnumerable<CmsItemResponse<AudioQuizItemResponse>>> AudioQuizzes { get; set; }
    
    [JsonProperty("video_quizzes")]
    public CmsResponse<IEnumerable<CmsItemResponse<VideoToAudioQuizItemResponse>>> VideoQuizzes { get; set; }
}
