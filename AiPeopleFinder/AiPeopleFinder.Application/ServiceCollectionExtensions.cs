﻿using Microsoft.Extensions.DependencyInjection;

namespace AiPeopleFinder.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPeopleFinderService, PeopleFinderService>();
        return services;
    }
}