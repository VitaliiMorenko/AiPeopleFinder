using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Infrastructure.AiServices;
using AiPeopleFinder.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Responses;

namespace AiPeopleFinder.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(_ => new OpenAIResponseClient("gpt-4o-mini",
            "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"));
        
        services.AddScoped<ISearchRequestDetailsRepository, DummySearchRequestDetailsRepository>();
        services.AddScoped<IAiPeopleInformationFinder, OpenAiPeopleInformationFinder>();
        
        return services;
    }
}