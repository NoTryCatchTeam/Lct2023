using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Responses.BaseCms;
using Lct2023.Business.RestServices.Base;
using Lct2023.Commons.Extensions;

namespace Lct2023.Business.Definitions;

public static class CmsRestServiceExtensions
{
    public static async Task<IEnumerable<TItemResult>> LoadUntilEndAsync<TItemResult, TRestService>(this TRestService restService, Func<TRestService, int, Task<CmsResponse<IEnumerable<TItemResult>>>> factory)
        where TRestService : ICmsBaseRestService
    {
        var list = new List<TItemResult>();
        var shouldLoad = true;
        
        while (shouldLoad)
        {
            var newPage = await factory(restService, list.Count);
            newPage?.Data?.Then(nP => list.AddRange(nP));
            shouldLoad = newPage?.Data?.Any() == true
                && list.Count < newPage?.Meta?.Pagination?.Total;
        }

        return list;
    }
}