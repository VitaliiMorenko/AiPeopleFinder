using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application.Repositories;

public interface ISearchRequestDetailsRepository
{
    Task CreateOrUpdate(SearchRequestDetails details);
    Task<SearchRequestDetails?> GetBySearchTerm(string searchTerm);
}