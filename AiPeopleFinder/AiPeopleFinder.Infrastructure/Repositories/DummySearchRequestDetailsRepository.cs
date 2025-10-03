using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Infrastructure.Repositories;

public class DummySearchRequestDetailsRepository : ISearchRequestDetailsRepository
{
    public void Save(SearchRequestDetails details)
    {
        // do nothing
    }

    public SearchRequestDetails GetBySearchTerm(string searchTerm)
    {
        return null;
    }
}