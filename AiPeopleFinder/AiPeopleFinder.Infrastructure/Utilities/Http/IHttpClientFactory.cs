namespace AiPeopleFinder.Infrastructure.Utilities.Http;

public interface IHttpClientFactory
{
    HttpClient CreateHttpClient(string? baseAddress = null);
}