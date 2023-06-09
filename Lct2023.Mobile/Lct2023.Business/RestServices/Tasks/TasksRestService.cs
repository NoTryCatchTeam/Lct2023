using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Tasks;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Tasks;

public class TasksRestService : BaseRestService, ITasksRestService
{
    public TasksRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseCmsPath)
    {
    }

    public Task<IEnumerable<CmsItemResponse<TaskItemResponse>>> GetTasksAsync(CancellationToken token) =>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<TaskItemResponse>>>("tasks?populate[quizzes][populate]=*&populate[video_quizzes][populate]=*&populate[audio_quizzes][populate]=*", HttpMethod.Get,
            token);

    public Task<IEnumerable<CmsItemResponse<TaskOfTheDayItemResponse>>> GetTaskOfTheDayAsync(CancellationToken token) =>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<TaskOfTheDayItemResponse>>>(
            $"daily-tasks?filters[date][$eq]={DateTimeOffset.UtcNow.Date:yyyy-MM-dd}&populate[task][populate][quizzes][populate]=*&populate[task][populate][video_quizzes][populate]=*&populate[task][populate][audio_quizzes][populate]=*",
            HttpMethod.Get,
            token);
}
