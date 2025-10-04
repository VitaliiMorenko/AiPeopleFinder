using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Infrastructure.AiServices;
using AiPeopleFinder.Infrastructure.Http;
using AiPeopleFinder.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AiPeopleFinder.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ISearchRequestDetailsRepository, DummySearchRequestDetailsRepository>();
        services.AddScoped<IAiPeopleInformationFinder, OpenAiPeopleInformationFinder>();
        services.AddScoped<IHttpClientFactory, HttpClientFactory>();
        
        return services;
    }
}