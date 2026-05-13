using System.Net;
using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace BuildingBlocks.Infrastructure.DataAccess.Cosmos
{
    public abstract class CosmosContext : ICosmosContext
    {
        private readonly IDictionary<Type, Container> _containers = new Dictionary<Type, Container>();
        private const int MinimumContainerThroughput = 400;
        protected readonly CosmosClient CosmosClient;
        public string DatabaseName { get; }
        
        public CosmosContext(CosmosConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.DatabaseName)) throw new ArgumentNullException(nameof(configuration.DatabaseName));
            if (string.IsNullOrEmpty(configuration.ConnectionString)) throw new ArgumentNullException(nameof(configuration.ConnectionString));

            DatabaseName = configuration.DatabaseName;
            var cosmosClientBuilder = new CosmosClientBuilder(configuration.ConnectionString)
                .WithSerializerOptions(new CosmosSerializationOptions() {PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase})
                .WithConnectionModeDirect();

            if (string.IsNullOrEmpty(configuration.Region) == false)
            {
                cosmosClientBuilder.WithApplicationRegion(configuration.Region);
            }

            CosmosClient = cosmosClientBuilder.Build();
        }
        
        
        
        protected void Map<TDocument>(string containerName)
        where TDocument: IDocument
        {
            _containers.TryAdd(typeof(TDocument), CosmosClient.GetContainer(DatabaseName, containerName));
        }

        private Container ContainerClient<TDocument>()
        {
            return _containers[typeof(TDocument)];
        }


        public async Task<List<TDocument>>FromSqlRaw<TDocument>(string query)
            where TDocument : class, IDocument
        {
            var result = (await ContainerClient<TDocument>().
                GetItemQueryIterator<CosmosRepositoryDocument<TDocument>>(query)
                .ToDocumentListAsync()).Result;

            return result.Select(z=>z.Document).ToList();
        }
        
        public IQueryable<TDocument> Query<TDocument>()
            where TDocument : class, IDocument
        {
            return ContainerClient<TDocument>().GetItemLinqQueryable<CosmosRepositoryDocument<TDocument>>()
                .Where(x => x.Type == typeof(TDocument).Name)
                .Select(x => x.Document);
        }

        public async Task<TDocument?> FindAsync<TDocument>(DocumentIdentity identity)
            where TDocument : class, IDocument
        {
            try
            {
                var result = await  ContainerClient<TDocument>().ReadItemAsync<CosmosRepositoryDocument<TDocument>>(identity.DocumentId, new PartitionKey(identity.PartitionKey));
                return result?.Resource?.Document;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }

        public async Task<TDocument> GetAsync<TDocument>(DocumentIdentity identity)
            where TDocument : class, IDocument
        {
            var document = await FindAsync<TDocument>(identity);
          
            if (document == null)
            {
                throw new Exception($"Document not found.");
            }

            return document;
        }

        public async Task AddAsync<TDocument>(TDocument document)
            where TDocument : class, IDocument
        {
            await ContainerClient<TDocument>().CreateItemAsync(CosmosRepositoryDocument<TDocument>.Create(document));
        }

        public async Task UpsertAsync<TDocument>(TDocument document)
            where TDocument : class?, IDocument?
        {
            await UpsertCoreAsync(document);
        }

        public async Task RemoveAsync<TDocument>(DocumentIdentity identity)
            where TDocument : class, IDocument
        {
            await ContainerClient<TDocument>().DeleteItemAsync<CosmosRepositoryDocument<TDocument>>(identity.Id, new PartitionKey(identity.PartitionKey));
        }

        public async Task RemoveAsync<TDocument>(TDocument item)
            where TDocument : class, IDocument
        {
            var document = CosmosRepositoryDocument<TDocument>.Create(item);
            await ContainerClient<TDocument>().DeleteItemAsync<CosmosRepositoryDocument<TDocument>>(document.Id, new PartitionKey(document.PartitionKey));
        }

        private async Task<ItemResponse<CosmosRepositoryDocument<TDocument>>> UpsertCoreAsync<TDocument>(TDocument document)
            where TDocument : class, IDocument
        {
            return await ContainerClient<TDocument>().UpsertItemAsync(CosmosRepositoryDocument<TDocument>.Create(document));
        }
    }
}