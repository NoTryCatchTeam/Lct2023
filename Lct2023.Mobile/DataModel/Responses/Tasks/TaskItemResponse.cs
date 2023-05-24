using System.Collections.Generic;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Quizzes;

namespace DataModel.Responses.Tasks;

public class TaskItemResponse
{
    public CmsResponse<IEnumerable<CmsItemResponse<QuizItemResponse>>> Quizzes { get; set; }
}
