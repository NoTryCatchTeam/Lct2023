using Newtonsoft.Json;

namespace DataModel.Responses.Quizzes;

public class QuizItemResponse
{
    public string Question { get; set; }

    [JsonProperty("a")]
    public string AnswerA { get; set; }

    [JsonProperty("b")]
    public string AnswerB { get; set; }

    [JsonProperty("c")]
    public string AnswerC { get; set; }

    [JsonProperty("d")]
    public string AnswerD { get; set; }

    [JsonProperty("answer")]
    public string CorrectAnswerValue { get; set; }

    public string Explanation { get; set; }
}
