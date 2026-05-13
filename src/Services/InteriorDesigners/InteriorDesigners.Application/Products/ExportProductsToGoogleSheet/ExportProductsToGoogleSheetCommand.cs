using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Abstractions.Excel;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InteriorDesigners.Application.Products.ExportProductsToGoogleSheet;


    public class ExportProductsToGoogleSheetCommand : ICommand
    {
        public required string InteriorDesignerCode { get; init; }
    }

    public class ExportProductsToGoogleSheetCommandHandler : ICommandHandler<ExportProductsToGoogleSheetCommand>
    {
        private readonly IInteriorDesignerProductsRepository _productsRepository;
        private readonly IInteriorDesignerRepository _interiorDesignerRepository;
        private readonly ILogger<ExportProductsToGoogleSheetCommand> _logger;
        private readonly IExcelDictionaryProvider _excelDictionaryProvider;


        public ExportProductsToGoogleSheetCommandHandler(IInteriorDesignerProductsRepository productsRepository, 
            IInteriorDesignerRepository interiorDesignerRepository, 
            ILogger<ExportProductsToGoogleSheetCommand> logger, 
            IExcelDictionaryProvider excelDictionaryProvider)
        {
            _productsRepository = productsRepository;
            _interiorDesignerRepository = interiorDesignerRepository;
            _logger = logger;
            _excelDictionaryProvider = excelDictionaryProvider;
        }

        public async Task<Unit> Handle(ExportProductsToGoogleSheetCommand request, CancellationToken cancellationToken)
        {
            var interiorDesigner = await _interiorDesignerRepository.FindAsync(request.InteriorDesignerCode);

            if (interiorDesigner == null)
            {
                throw new Exception("Interior Designer Not Found.");
            }
            
            var products = await _productsRepository.FindAll(x => x.InteriorDesignerCode == interiorDesigner.Code);

            var columnRows = products.Select(GetAllPropertyValues).ToList();
            await _excelDictionaryProvider.ExportAsync(interiorDesigner.SpreadSheetId, GetAllPropertyNames(),columnRows);
            return Unit.Value;
        }
        

        private IList<object> GetAllPropertyValues(ProductAggregate product)
        {
            var values = new List<object>
            {
                product.Id,
                product.CoreProductData.Name,
                product.DisplayName,
                product.Description,
                product.IsEnabled,
                product.ShowLabel,
                product.Price
            };

            return values;
        }

        private List<object> GetAllPropertyNames()
        {
            var properties = new List<object>
            {
                nameof(ProductAggregate.Id),
                $"{nameof(ProductAggregate.CoreProductData)}{nameof(CoreProductData.Name)}",
                nameof(ProductAggregate.DisplayName),
                nameof(ProductAggregate.Description),
                nameof(ProductAggregate.IsEnabled),
                nameof(ProductAggregate.ShowLabel),
                nameof(ProductAggregate.Price),
                $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Name)}",
                $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.ProductLink)}",
                $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Link)}",
                $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Logo)}",
                $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Contact)}{nameof(Contact.Email)}",
                $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Contact)}{nameof(Contact.PhoneNumber)}"
            };

            return properties;
        }
    }
