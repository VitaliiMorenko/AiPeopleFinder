namespace AiPeopleFinder.Application.AiServices;

public interface IAiPeopleInformationFinder
{
    Task<string> SearchInformation(string searchTerm);
}