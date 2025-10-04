namespace AiPeopleFinder.Infrastructure.Http;

public interface IHttpClientFactory
{
    HttpClient CreateHttpClient(string? baseAddress = null);
}