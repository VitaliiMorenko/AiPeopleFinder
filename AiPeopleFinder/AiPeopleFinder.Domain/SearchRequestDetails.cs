namespace AiPeopleFinder.Domain;

public class SearchRequestDetails
{
    public string SearchTerm { get; set; }
    
    public string Response { get; set; }
    
    public bool IsCached { get; private set; }

    public void MarkCached()
    {
        IsCached = true;
    }

    public void InvalidateCache()
    {
        IsCached = false;
    }
}