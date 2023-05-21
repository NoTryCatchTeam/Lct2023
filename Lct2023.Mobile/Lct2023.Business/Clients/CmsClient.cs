using System.Net.Http;

namespace Lct2023.Business.Clients;

public class CmsClient : HttpClient
{
    public CmsClient(HttpMessageHandler handler)
        : base(handler)
    {
    }
}
