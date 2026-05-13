using System.Net;
using System.Text.Json;
using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Abstractions.Storage;
using BuildingBlocks.Application.EventBus;
using BuildingBlocks.Application.Exceptions.Exceptions;
using CoreProducts.Application.Products.Dtos;
using CoreProducts.Application.Products.IntegrationEvents.Events;
using CoreProducts.Application.Products.IntegrationEvents.Events.External;
using CoreProducts.Application.Products.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoreProducts.Application.Products.Commands;

public class ImportProductsFromFileCommand : ICommand
{
    public string FileUrl { get; }

    public ImportProductsFromFileCommand(string fileUrl)
    {
        FileUrl = fileUrl;
    }
}

public class ImportProductsFromFileCommandHandler : ICommandHandler<ImportProductsFromFileCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventBus _eventBus;
    private readonly ICategoryProvider _categoryProvider;
    private readonly IBlobFileProvider _blobFileProvider;
    private readonly ILogger<ImportProductsFromFileCommandHandler> _logger;

    public ImportProductsFromFileCommandHandler(
        IEventBus eventBus, 
        ICategoryProvider categoryProvider,
        IProductRepository productRepository,
        IBlobFileProvider blobFileProvider,
        ILogger<ImportProductsFromFileCommandHandler> logger)
    {
        _eventBus = eventBus;
        _categoryProvider = categoryProvider;
        _productRepository = productRepository;
        _blobFileProvider = blobFileProvider;
        _logger = logger;
    }

    public async Task<Unit> Handle(ImportProductsFromFileCommand request, CancellationToken cancellationToken)
    {
        if (request.FileUrl.Contains("Imported"))
        {
            return Unit.Value;
        }
        
        List<ProductDto>? products = null;
        await using (var stream = await _blobFileProvider.OpenReadAsync(request.FileUrl))
        {
            products =
                await JsonSerializer.DeserializeAsync<List<ProductDto>>(stream,
                    cancellationToken: cancellationToken, options: new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }


        if (products == null)
            throw new CustomException("Can not read product file from storage.", HttpStatusCode.InternalServerError);

        _logger.LogInformation("Import {ProductsCount} products", products.Count);

        foreach (var productItem in products)
        {
            var product = await productItem.MapToDomainModel(productItem.Id, _categoryProvider);
            await _productRepository.UpsertAsync(product, cancellationToken);
        }
        
        await _eventBus.PublishAsync(new CoreProductsUpdatedIntegrationEvent());
        await _blobFileProvider.MoveAsync(request.FileUrl,"Imported",$"{DateTime.UtcNow:yyyy_MM_dd_HH_mm}");

        return Unit.Value;
    }
}