using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application;

public interface IPeopleFinderService
{
    Task<PersonProfile?> GetSearchRequestDetails(string searchTerm);
}