namespace AiPeopleFinder.Domain;

public class SearchRequestDetails
{
    public string SearchTerm { get; init; }
    
    public PersonProfile? Profile { get; init; }
}