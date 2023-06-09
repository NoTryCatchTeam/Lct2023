using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DataModel.Responses.Users;
using Lct2023.Business.Helpers;
using Lct2023.Business.RestServices.Base;

namespace Lct2023.Business.RestServices.Users;

public class UsersRestService : BaseRestService, IUsersRestService
{
    public UsersRestService(HttpClient httpClient, IRequestAuthenticator requestAuthenticator)
        : base(httpClient, requestAuthenticator, BusinessInit.Instance.BaseApiPath)
    {
    }

    public Task<UserItemResponse> GetSelfAsync(CancellationToken token) =>
        ExecuteAsync<UserItemResponse>("users/self", HttpMethod.Get, token);

    public Task<int> GetRatingAsync(CancellationToken token) =>
        ExecuteAsync<int>("users/rating", HttpMethod.Get, token);

    public Task<int> UpdateRatingAsync(int increment, CancellationToken token) =>
        ExecuteAsync<int, int>("users/rating", increment, HttpMethod.Post, token);
}
