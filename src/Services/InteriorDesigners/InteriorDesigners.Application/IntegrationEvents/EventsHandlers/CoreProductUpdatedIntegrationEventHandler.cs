using BuildingBlocks.Application.EventBus.Events;
using InteriorDesigners.Application.IntegrationEvents.Events;
using InteriorDesigners.Application.Products.ExportProductsToGoogleSheet;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace InteriorDesigners.Application.IntegrationEvents.EventsHandlers;

public class CoreProductUpdatedIntegrationEventHandler
    : IntegrationEventHandler<CoreProductsUpdatedIntegrationEvent>
{
    private readonly IInteriorDesignerRepository _interiorDesignerRepository;
    private readonly IInteriorDesignerProductsRepository _productsRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CoreProductUpdatedIntegrationEventHandler> _logger;
    private readonly ICoreProductsServiceClient _productsService;
    private readonly IMediator _mediator;

    public CoreProductUpdatedIntegrationEventHandler(ILoggerFactory loggerFactory, 
        IInteriorDesignerRepository interiorDesignerRepository,
        IMemoryCache memoryCache,
        ILogger<CoreProductUpdatedIntegrationEventHandler> logger, 
        IInteriorDesignerProductsRepository productsRepository,
        ICoreProductsServiceClient productsService, IMediator mediator,
        Tracer tracer) : 
        base(loggerFactory, tracer)
    {
        _interiorDesignerRepository = interiorDesignerRepository;
        _memoryCache = memoryCache;
        _logger = logger;
        _productsRepository = productsRepository;
        _productsService = productsService;
        _mediator = mediator;
    }

    protected override async Task HandleEvent(CoreProductsUpdatedIntegrationEvent @event, 
        CancellationToken cancellationToken)
    {
        var products = await _productsService.GetProducts();
        
        var interiorDesigners = await _memoryCache.GetOrCreateAsync("interior_designers",
            entry => _interiorDesignerRepository.GetAllAsync());

        foreach (var productDto in products)
        {
            foreach (var interiorDesigner in interiorDesigners)
            {
                var productId = productDto.Id;
                var product = await _productsRepository.FindAsync(new ProductIdentity(productId,interiorDesigner.Code));

                if (product == null)
                {
                    _logger.LogInformation("Product {ProductId} not exists", productId);
                    product = productDto.Map(new ProductAggregate(), interiorDesigner.Code);
                }
                else
                {
                    _logger.LogInformation("Updating Product {@ProductId}. {@Event}", productId, @event);
                    product = productDto.Map(product, interiorDesigner.Code);
                }

                product.IsEnabled = productDto.Designers.Select(x => x.ToUpper()).Contains(interiorDesigner.Code.ToUpper());
                
                await _productsRepository.UpsertAsync(product, new CancellationToken());
            }
        }

        foreach (var interiorDesigner in interiorDesigners)
        {
            if (products.Any())
            {
                await _mediator.Send(new ExportProductsToGoogleSheetCommand
                {
                    InteriorDesignerCode = interiorDesigner.Code
                });
            }
        }
    }
}