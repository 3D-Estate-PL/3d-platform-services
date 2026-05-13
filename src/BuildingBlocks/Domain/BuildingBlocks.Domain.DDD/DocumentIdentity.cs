namespace BuildingBlocks.Domain.DDD;

public abstract class DocumentIdentity
{
    public string Id { get; protected set; }
    public abstract string DocumentId { get; }
    public abstract string PartitionKey { get; }
}

public class DocumentIdentity<TDocument> : DocumentIdentity
    where TDocument : IDocument
        
{
    public override string DocumentId => $"{PartitionKey}|{Id}";
    public override string PartitionKey => $"{typeof(TDocument).Name}";

    public DocumentIdentity(string id)
    {
        Id = id;
    }
}