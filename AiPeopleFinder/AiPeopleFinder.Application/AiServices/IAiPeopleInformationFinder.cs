using AiPeopleFinder.Domain;

namespace AiPeopleFinder.Application.AiServices;

public interface IAiPeopleInformationFinder
{
    Task<PersonProfile?> SearchInformation(string searchTerm);
}