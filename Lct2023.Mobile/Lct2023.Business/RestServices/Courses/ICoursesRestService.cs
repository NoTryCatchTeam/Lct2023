using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Courses;

namespace Lct2023.Business.RestServices.Courses;

public interface ICoursesRestService
{
    Task<IEnumerable<CmsItemResponse<CourseItemResponse>>> GetCoursesAsync(CancellationToken token);
}
