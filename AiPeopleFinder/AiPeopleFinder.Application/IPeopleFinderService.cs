using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application;

public interface IPeopleFinderService
{
    Task<SearchRequestDetails?> GetSearchRequestDetails(string searchTerm);
}