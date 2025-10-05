using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application;

public interface IPeopleFinderService
{
    Task<SearchRequestDetails?> GetSearchRequestDetails(string searchTerm);
    Task<List<string>> GetSearchTermSuggestions(string searchTerm, int limit);
}