using AiPeopleFinder.Application.Repositories;
using AiPeopleFinder.Domain;
using AiPeopleFinder.Infrastructure.Configuration;
using AiPeopleFinder.Infrastructure.Entities;
using AiPeopleFinder.Infrastructure.Utilities.DateTime;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AiPeopleFinder.Infrastructure.Repositories;

public class SearchRequestDetailsRepository : ISearchRequestDetailsRepository
{
    private readonly IMongoCollection<SearchRequestDetailsEntity> _collection;
    private readonly IDateTimeService _dateTimeService;
    private readonly Config _config;

    public SearchRequestDetailsRepository(IMongoDatabase db,
        IDateTimeService dateTimeService,
        IOptions<Config> options)
    {
        _dateTimeService = dateTimeService;
        _collection = db.GetCollection<SearchRequestDetailsEntity>(nameof(SearchRequestDetailsEntity));
        _config = options.Value;
        
        var expiresAtIndex = new CreateIndexModel<SearchRequestDetailsEntity>(
            Builders<SearchRequestDetailsEntity>.IndexKeys.Ascending(x => x.ExpiresAt), new CreateIndexOptions { Unique = false });
        var searchTermIndex = new CreateIndexModel<SearchRequestDetailsEntity>(
            Builders<SearchRequestDetailsEntity>.IndexKeys.Ascending(x => x.SearchTerm), new CreateIndexOptions { Unique = true });
        _collection.Indexes.CreateMany([expiresAtIndex, searchTermIndex]);
    }
    
    public async Task CreateOrUpdate(SearchRequestDetails details)
    {
        var id = GetIdFromSearchTerm(details.SearchTerm);
        var now = _dateTimeService.UtcNow;
        var filter = Builders<SearchRequestDetailsEntity>.Filter.Eq(x => x.Id, id);
        
        var upsert = Builders<SearchRequestDetailsEntity>.Update
            .Set(x => x.SearchTerm, details.SearchTerm)
            .Set(x => x.Profile, details.Profile)
            .Set(x => x.UpdatedAt, now)
            .Set(x => x.ExpiresAt, now.Add(TimeSpan.FromMinutes(_config.CacheTtlInMinutes)))
            .Set(x => x.CreatedAt, now);
        
        var options = new UpdateOptions { IsUpsert = true };
        await _collection.UpdateOneAsync(filter, upsert, options);
    }

    public async Task<SearchRequestDetails?> GetBySearchTerm(string searchTerm)
    {
        var id = GetIdFromSearchTerm(searchTerm);
        var now = _dateTimeService.UtcNow;
        
        var item = await _collection.Find(x => x.Id == id && x.ExpiresAt > now).FirstOrDefaultAsync();
        if (item == null)
            return null;

        return new SearchRequestDetails
        {
            SearchTerm = item.SearchTerm,
            Profile = item.Profile
        };
    }

    private static string GetIdFromSearchTerm(string searchTerm)
    {
        return searchTerm.Trim().ToLower();
    }
}