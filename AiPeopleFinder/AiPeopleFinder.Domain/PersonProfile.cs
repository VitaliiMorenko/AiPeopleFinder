namespace AiPeopleFinder.Domain;

public class PersonProfile(
    string name,
    string company,
    string currentRole,
    List<string> keyFacts,
    string pastRolesCompanies)
{
    public string Name { get; } = name;
    public string Company { get; } = company;
    public string CurrentRole { get; } = currentRole;
    public List<string> KeyFacts { get; } = keyFacts;
    public string PastRolesCompanies { get; } = pastRolesCompanies;
}