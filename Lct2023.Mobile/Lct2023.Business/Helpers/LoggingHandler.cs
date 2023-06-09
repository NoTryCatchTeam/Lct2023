using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Lct2023.Business.Helpers;

public class LoggingHandler : HttpClientHandler
{
    private readonly string _baseUrl;
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(string baseUrl, ILogger<LoggingHandler> logger)
    {
        _baseUrl = baseUrl;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid();

        var requestString = new StringBuilder($"Request {requestId}")
            .AppendLine()
            .AppendLine($"Time: {DateTimeOffset.Now:HH:mm:ss}")
            .AppendLine($"{request.Method} :: {request.RequestUri}");

        foreach (var header in request.Headers)
        {
            requestString.AppendLine($"{header.Key}: {header.Value.FirstOrDefault()}");
        }

        if (request.Content != null)
        {
            requestString.AppendLine(await request.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        _logger.LogDebug(requestString.ToString());

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var responseString = new StringBuilder($"Response {requestId}")
            .AppendLine()
            .AppendLine($"Time: {DateTimeOffset.Now:HH:mm:ss}")
            .AppendLine($"{request.Method} :: {request.RequestUri}")
            .AppendLine();

        if (response.Content != null)
        {
            requestString.AppendLine(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        _logger.LogDebug(responseString.ToString());

        return response;
    }
}
