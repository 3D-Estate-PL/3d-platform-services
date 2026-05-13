using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Model;
using Newtonsoft.Json;

namespace BuildingBlocks.Infrastructure.DataAccess.Cosmos.Repositories
{
    internal class CosmosRepositoryDocument<TDocument> : CosmosDocument
        where TDocument : class, IDocument
    {
        [JsonProperty("document")]
        public TDocument Document { get; private set; }

        public static CosmosRepositoryDocument<TDocument> Create(TDocument document)
        {
            var documentIdentity = document.GetIdentity();
            
            return new CosmosRepositoryDocument<TDocument>()
            {
                Id = documentIdentity.DocumentId,
                PartitionKey = documentIdentity.PartitionKey,
                Type = typeof(TDocument).Name,
                Ttl = -1,
                Document = document
            };
        }
    }
}