using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<CosmosRepositoryResult<T>> ToDocumentListAsync<T>(this IQueryable<T> query)
        {
            var requestCharge = 0.0;
            var elapsedTime = TimeSpan.Zero;
            var result = new List<T>();

            var iterator = query.ToFeedIterator();

            while (iterator.HasMoreResults)
            {
                var feedResponse = await iterator.ReadNextAsync();

                result.AddRange(feedResponse.ToList());
                requestCharge = requestCharge + feedResponse.RequestCharge;
                elapsedTime = elapsedTime.Add(feedResponse.Diagnostics.GetClientElapsedTime());
            }

            return new CosmosRepositoryResult<T>(result);
        }
        
        
        public static async Task<CosmosRepositoryResult<T>> ToDocumentListAsync<T>(this FeedIterator<T> iterator)
        {
            var requestCharge = 0.0;
            var elapsedTime = TimeSpan.Zero;
            var result = new List<T>();
            
            while (iterator.HasMoreResults)
            {
                var feedResponse = await iterator.ReadNextAsync();

                result.AddRange(feedResponse.ToList());
                requestCharge = requestCharge + feedResponse.RequestCharge;
                elapsedTime = elapsedTime.Add(feedResponse.Diagnostics.GetClientElapsedTime());
            }

            return new CosmosRepositoryResult<T>(result);
        }
    }
}