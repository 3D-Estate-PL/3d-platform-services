using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Model;

namespace BuildingBlocks.Infrastructure.DataAccess.Cosmos
{
    public interface ICosmosContext
    {
        public Task<List<TDocument>> FromSqlRaw<TDocument>(string query)
            where TDocument : class, IDocument;
        
        IQueryable<TDocument> Query<TDocument>()
            where TDocument : class, IDocument;

        Task<TDocument?> FindAsync<TDocument>(DocumentIdentity identity)
            where TDocument : class, IDocument;

        Task<TDocument> GetAsync<TDocument>(DocumentIdentity identity)
            where TDocument : class, IDocument;

        Task AddAsync<TDocument>(TDocument document)
            where TDocument : class, IDocument;

        Task UpsertAsync<TDocument>(TDocument document)
            where TDocument : class, IDocument;
        
        Task RemoveAsync<TDocument>(TDocument item)
            where TDocument : class, IDocument;

        Task RemoveAsync<TDocument>(DocumentIdentity identity)
            where TDocument : class, IDocument;
    }
}