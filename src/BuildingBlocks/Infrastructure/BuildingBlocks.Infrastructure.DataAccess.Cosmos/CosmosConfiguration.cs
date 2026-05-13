namespace BuildingBlocks.Infrastructure.DataAccess.Cosmos;

public abstract record CosmosConfiguration : ICosmosConfiguration
{
    public  string ConnectionString { get; set; }
    public string DatabaseName { get; set; } = "platform-db";
    public  string Region { get; set; }
    public abstract string ConfigurationName { get; }
}