using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Stories;

namespace Lct2023.Business.RestServices.Stories;

public interface IStoriesRestService
{
    
    Task<IEnumerable<CmsItemResponse<StoryQuizResponse>>> GetStoryQuizzesAsync(CancellationToken token);
}