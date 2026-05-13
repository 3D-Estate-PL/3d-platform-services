using BuildingBlocks.Application.Exceptions.Exceptions;
using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using Microsoft.Azure.Cosmos;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;

public interface ITourStyleConfigurationRepository
{
    Task<ConfigurationOwner> GetOwnerAsync(ConfigurationOwnerIdentity id);

    Task<ConfigurationOwner?> FindOwnerByEmailAsync(string email);


    Task<ConfigurationOwner> GetOwnerByEmailAsync(string email);


    Task UpsertOwnerAsync(ConfigurationOwner configuration);

    Task<TourStyleConfiguration> GetConfigurationAsync(string id);
    Task UpsertConfigurationAsync(TourStyleConfiguration configuration);
    Task<TourStyleConfiguration> FindConfigurationByCodeAsync(string ownerId, string code);

    Task<List<TourStyleConfiguration>> FindConfigurationsByOwnerAsync(string ownerId);


    Task RemoveAsync<TDocument>(DocumentIdentity identity)
        where TDocument : class, IDocument;

    Task<TourStyleConfiguration> GetConfigurationByCodeAsync(string code);
}

public class TourStyleConfigurationRepository : ITourStyleConfigurationRepository
{
    private readonly ToursStylesConfiguratorDbContext _context;


    public TourStyleConfigurationRepository(ToursStylesConfiguratorDbContext context)
    {
        _context = context;
    }

    public async Task<ConfigurationOwner> GetOwnerAsync(ConfigurationOwnerIdentity id)
    {
        try
        {
            var response = await _context.FindAsync<ConfigurationOwner>(id);
            return response;
        }
        catch (CosmosException e) //For handling item not found and other exceptions
        {
            return null;
        }
    }

    public async Task<ConfigurationOwner?> FindOwnerByEmailAsync(string email)
    {
        try
        {
            var result = await _context.Query<ConfigurationOwner>()
                .Where(x => x.Email == email).ToDocumentListAsync();

            return result.Result.SingleOrDefault();
        }
        catch (CosmosException e) //For handling item not found and other exceptions
        {
            return null;
        }
    }

    public async Task<ConfigurationOwner> GetOwnerByEmailAsync(string email)
    {
        var result = await FindOwnerByEmailAsync(email);

        if (result == null) throw new CustomException("Owner Not Found.");

        return result;
    }

    public async Task UpsertOwnerAsync(ConfigurationOwner owner)
    {
        await _context.UpsertAsync(owner);
    }

    public async Task<TourStyleConfiguration> GetConfigurationAsync(string id)
    {
        try
        {
            var result = await _context.Query<TourStyleConfiguration>()
                .Where(x => x.Id == id).ToDocumentListAsync();


            var entity = result.Result.SingleOrDefault();
            if (entity == null) throw new Exception("Configuration Not Found.");

            return entity;
        }
        catch (CosmosException e) //For handling item not found and other exceptions
        {
            throw new Exception("Configuration Not Found.");
        }
    }

    public async Task UpsertConfigurationAsync(TourStyleConfiguration configuration)
    {
        await _context.UpsertAsync(configuration);
    }

    public async Task<TourStyleConfiguration> FindConfigurationByCodeAsync(string ownerId, string code)
    {
        try
        {
            var result = await _context.Query<TourStyleConfiguration>()
                .Where(x => x.OwnerId == ownerId && x.Code == code).ToDocumentListAsync();

            return result.Result.SingleOrDefault();
        }
        catch (CosmosException e) //For handling item not found and other exceptions
        {
            throw new Exception("Configuration Not Found.");
        }
    }

    public async Task<List<TourStyleConfiguration>> FindConfigurationsByOwnerAsync(string ownerId)
    {
        try
        {
            var result = await _context.Query<TourStyleConfiguration>()
                .Where(x => x.OwnerId == ownerId).ToDocumentListAsync();

            return result.Result.ToList();
        }
        catch (CosmosException e) //For handling item not found and other exceptions
        {
            throw new Exception("Configurations Not Found.");
        }
    }

    public async Task RemoveAsync<TDocument>(DocumentIdentity identity)
        where TDocument : class, IDocument
    {
        await _context.RemoveAsync<TDocument>(identity);
    }

    public async Task<TourStyleConfiguration> GetConfigurationByCodeAsync(string code)
    {
        try
        {
            var result = await _context.Query<TourStyleConfiguration>()
                .Where(x => x.Code == code).ToDocumentListAsync();


            var entity = result.Result.SingleOrDefault();
            if (entity == null) throw new Exception("Configuration Not Found.");

            return entity;
        }
        catch (CosmosException e) //For handling item not found and other exceptions
        {
            throw new Exception("Configuration Not Found.");
        }
    }
}