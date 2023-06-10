using DataModel.Responses.BaseCms;

namespace DataModel.Responses.Tasks;

public class TaskOfTheDayItemResponse
{
    public CmsResponse<CmsItemResponse<TaskItemResponse>> Task { get; set; }
}
