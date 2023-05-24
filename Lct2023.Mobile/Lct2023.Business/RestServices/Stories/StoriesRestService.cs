using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Stories;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Stories;

public class StoriesRestService : BaseRestService, IStoriesRestService
{
    public StoriesRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseCmsPath)
    {
    }

    public Task<IEnumerable<CmsItemResponse<StoryQuizResponse>>> GetStoryQuizzesAsync(CancellationToken token) =>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<StoryQuizResponse>>>("story-quizzes", HttpMethod.Get, token);
}
