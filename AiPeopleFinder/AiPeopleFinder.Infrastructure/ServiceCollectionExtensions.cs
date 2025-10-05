using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Infrastructure.AiServices;
using AiPeopleFinder.Infrastructure.Repositories;
using AiPeopleFinder.Infrastructure.Utilities.DateTime;
using AiPeopleFinder.Infrastructure.Utilities.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AiPeopleFinder.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISearchRequestDetailsRepository, SearchRequestDetailsRepository>();
        services.AddScoped<IAiPeopleInformationFinder, OpenAiPeopleInformationFinder>();
        services.AddScoped<IHttpClientFactory, HttpClientFactory>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        
        services.AddSingleton<IMongoClient>(cl => new MongoClient(configuration["Config:MongoDB:ConnectionString"]));
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(configuration["Config:MongoDB:DatabaseName"]);
        });
        
        return services;
    }
}