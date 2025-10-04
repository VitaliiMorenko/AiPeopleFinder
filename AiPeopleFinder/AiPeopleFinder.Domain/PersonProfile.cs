namespace AiPeopleFinder.Domain;

public sealed record PersonProfile(
    string Name,
    string Company,
    string CurrentRole,
    List<string> KeyFacts,
    string PastRolesCompanies);