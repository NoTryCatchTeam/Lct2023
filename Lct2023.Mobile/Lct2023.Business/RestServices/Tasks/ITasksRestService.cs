using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Tasks;

namespace Lct2023.Business.RestServices.Tasks;

public interface ITasksRestService
{
    Task<IEnumerable<CmsItemResponse<TaskItemResponse>>> GetTasksAsync(CancellationToken token);

    Task<IEnumerable<CmsItemResponse<TaskOfTheDayItemResponse>>> GetTaskOfTheDayAsync(CancellationToken token);
}
