using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application.Repositories;

public interface ISearchRequestDetailsRepository
{
    void Save(SearchRequestDetails details);
    SearchRequestDetails GetBySearchTerm(string searchTerm);
}