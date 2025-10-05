using AiPeopleFinder.Application.AiServices;
using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application;

public class PeopleFinderService(
    IAiPeopleInformationFinder aiPeopleInformationFinder,
    ISearchRequestDetailsRepository searchRequestDetailsRepository)
    : IPeopleFinderService
{
    
    public async Task<PersonProfile?> GetSearchRequestDetails(string searchTerm)
    {
        var cachedResult = await searchRequestDetailsRepository.GetBySearchTerm(searchTerm);
        if (cachedResult != null)
        {
            return cachedResult.Profile;
        }
        
        var aiSearchResult = await aiPeopleInformationFinder.SearchInformation(searchTerm);
        var result = new SearchRequestDetails
        {
            SearchTerm = searchTerm,
            Profile = aiSearchResult
        };
        
        await searchRequestDetailsRepository.CreateOrUpdate(result);
        return result.Profile;
    }
}