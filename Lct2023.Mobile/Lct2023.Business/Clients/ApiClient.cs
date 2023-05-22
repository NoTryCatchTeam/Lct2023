using System.Net.Http;

namespace Lct2023.Business.Clients;

public class ApiClient : HttpClient
{
    public ApiClient(HttpMessageHandler handler)
        : base(handler)
    {
    }
}
