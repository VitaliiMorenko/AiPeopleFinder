using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application;

public class PeopleFinderService(
    IAiPeopleInformationFinder aiPeopleInformationFinder,
    ISearchRequestDetailsRepository searchRequestDetailsRepository)
    : IPeopleFinderService
{
    
    public async Task<SearchRequestDetails?> GetSearchRequestDetails(string searchTerm)
    {
        var cachedResult = await searchRequestDetailsRepository.GetBySearchTerm(searchTerm);
        if (cachedResult != null)
        {
            return cachedResult;
        }
        
        var aiSearchResult = await aiPeopleInformationFinder.SearchInformation(searchTerm);
        var result = new SearchRequestDetails
        {
            SearchTerm = searchTerm,
            Profile = aiSearchResult
        };
        
        await searchRequestDetailsRepository.CreateOrUpdate(result);
        return result;
    }

    public async Task<List<string>> GetSearchTermSuggestions(string searchTerm, int limit)
    {
        return await searchRequestDetailsRepository.GetSearchTermSuggestions(searchTerm, limit);
    }
}