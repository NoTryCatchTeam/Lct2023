using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Stories;
using Lct2023.Business.Clients;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Stories;

public class StoriesRestService : BaseRestService, IStoriesRestService
{
    public StoriesRestService(CmsClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator)
    {
    }
    
    public Task<IEnumerable<CmsItemResponse<StoryQuizResponse>>> GetStoryQuizzesAsync(CancellationToken token)=>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<StoryQuizResponse>>>("story-quizzes", HttpMethod.Get, token);
}