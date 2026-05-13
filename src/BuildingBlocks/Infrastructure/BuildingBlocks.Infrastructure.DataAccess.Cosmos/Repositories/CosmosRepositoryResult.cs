namespace BuildingBlocks.Infrastructure.DataAccess.Cosmos.Repositories
{
    public class CosmosRepositoryResult<TDocument>
    {
        public List<TDocument> Result { get; }

        public CosmosRepositoryResult(TDocument result)
        {
            Result = new List<TDocument>() {result};
        }

        public CosmosRepositoryResult(List<TDocument> result)
        {
            Result = result;
        }

        public static CosmosRepositoryResult<TDocument> CreateEmpty()
        {
            return new CosmosRepositoryResult<TDocument>(new List<TDocument>());
        }
    }
}