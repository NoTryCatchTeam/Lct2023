using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Courses;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Courses;

public class CoursesRestService : BaseRestService, ICoursesRestService
{
    public CoursesRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseCmsPath)
    {
    }

    public Task<IEnumerable<CmsItemResponse<CourseItemResponse>>> GetCoursesAsync(CancellationToken token) =>
        CmsExecuteAsync<IEnumerable<CmsItemResponse<CourseItemResponse>>>("courses?[populate]=*&populate[lessons][populate]=*", HttpMethod.Get, token);
}