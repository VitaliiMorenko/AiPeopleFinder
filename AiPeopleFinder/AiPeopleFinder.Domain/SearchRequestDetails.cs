namespace AiPeopleFinder.Domain;

public class SearchRequestDetails
{
    public string SearchTerm { get; set; }
    
    public PersonProfile? Profile { get; set; }
}