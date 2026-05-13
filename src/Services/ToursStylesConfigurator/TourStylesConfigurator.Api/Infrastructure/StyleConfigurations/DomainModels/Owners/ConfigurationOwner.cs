using BuildingBlocks.Domain.DDD;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

public class ConfigurationOwner : IDocument
{
    public static ConfigurationOwner New(string name, string email)
    {
        return new ConfigurationOwner
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Email = email
        };
    }

    protected ConfigurationOwner()
    {
        
    }

    public string Id { get; set; }
    public  string Name { get; set; }
    public string Email { get;  set; }

    public DocumentIdentity GetIdentity()
    {
        return new ConfigurationOwnerIdentity(Id);
    }
}