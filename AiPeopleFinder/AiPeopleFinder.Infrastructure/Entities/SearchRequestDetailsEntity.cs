using AiPeopleFinder.Domain;
using MongoDB.Bson.Serialization.Attributes;

namespace AiPeopleFinder.Infrastructure.Entities;

public class SearchRequestDetailsEntity
{
    [BsonId]
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string SearchTerm { get; set; }
    public PersonProfile Profile { get; set; }
}