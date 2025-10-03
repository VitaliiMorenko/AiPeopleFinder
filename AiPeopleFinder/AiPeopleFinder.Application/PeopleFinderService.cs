using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application;

public class PeopleFinderService(
    IAiPeopleInformationFinder aiPeopleInformationFinder,
    ISearchRequestDetailsRepository searchRequestDetailsRepository)
    : IPeopleFinderService
{
    
    public async Task<SearchRequestDetails> GetSearchRequestDetails(string searchTerm)
    {
        var cachedResult = searchRequestDetailsRepository.GetBySearchTerm(searchTerm);
        if (cachedResult?.IsCached == true)
        {
            return cachedResult;
        }
        
        var aiSearchResult = await aiPeopleInformationFinder.SearchInformation(searchTerm);
        var result = new SearchRequestDetails
        {
            SearchTerm = searchTerm,
            Response = aiSearchResult
        };
        result.MarkCached();
        
        searchRequestDetailsRepository.Save(result);
        return result;
    }
}