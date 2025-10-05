namespace AiPeopleFinder.Infrastructure.Utilities.Http;

public class HttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateHttpClient(string? baseAddress = null)
    {
        var client = new HttpClient();
        if (baseAddress is not null)
        {
            client.BaseAddress = new Uri(baseAddress);
        }

        return client;
    }
}