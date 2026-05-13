using Newtonsoft.Json;

namespace BuildingBlocks.Infrastructure.DataAccess.Cosmos.Model
{
    public class CosmosDocument
    {
        public string Id { get; protected set; }
        [JsonProperty("PartitionKey")]
        public string PartitionKey { get; protected set; }
        public string Type { get; protected set; }

        public int? Ttl { get; set; }

        protected CosmosDocument()
        {
        }

        public CosmosDocument(string id, string partitionKey, string type)
        {
            Id = id;
            PartitionKey = partitionKey;
            Type = type;
        }
    }
}