using System.Dynamic;
using System.Globalization;
using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Abstractions.Excel;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InteriorDesigners.Application.Products.ImportProducts;


    public class ImportProductsFromGoogleSheetCommand : ICommand
    {
        public required string InteriorDesignerCode { get; init; }
    }

    public class ImportProductsFromGoogleSheetCommandHandler : ICommandHandler<ImportProductsFromGoogleSheetCommand>
    {
        private readonly IInteriorDesignerProductsRepository _productsRepository;
        private readonly IInteriorDesignerRepository _interiorDesignerRepository;
        private readonly ILogger<ImportProductsFromGoogleSheetCommandHandler> _logger;
        private readonly IExcelDictionaryProvider _excelDictionaryProvider;

        public ImportProductsFromGoogleSheetCommandHandler(IInteriorDesignerProductsRepository productsRepository,
            IInteriorDesignerRepository interiorDesignerRepository, 
            ILogger<ImportProductsFromGoogleSheetCommandHandler> logger,
            IExcelDictionaryProvider excelDictionaryProvider)
        {
            _productsRepository = productsRepository;
            _interiorDesignerRepository = interiorDesignerRepository;
            _logger = logger;
            _excelDictionaryProvider = excelDictionaryProvider;
        }


        public async Task<Unit> Handle(ImportProductsFromGoogleSheetCommand request, CancellationToken cancellationToken)
        {
            var interiorDesigner = await _interiorDesignerRepository.FindAsync(request.InteriorDesignerCode);

            if (interiorDesigner == null)
            {
                throw new Exception("Interior Designer Not Found.");
            }
            
            
            var gsp = new SheetParameters()
            {
                RangeColumnStart = 1,
                RangeRowStart = 1, 
                RangeColumnEnd = 14,
                RangeRowEnd = 1500,
                SpreadSheetId = interiorDesigner.SpreadSheetId
            };
            
            var rowValues = await _excelDictionaryProvider.Import(interiorDesigner.SheetName,gsp);
            
            foreach (IDictionary<String, dynamic> rowValue in rowValues)
            {
                var productId = rowValue["Id"] as string;

                if (productId == null)
                {
                    continue;
                }
                
                var product = await _productsRepository.FindAsync(new ProductIdentity(productId,interiorDesigner.Code));
                if (product != null)
                {
                    _logger.LogInformation("Update Product {ProductId}", productId);
                    
                    if(rowValue.Keys.Any(z=>z == nameof(ProductAggregate.DisplayName)))
                         product.DisplayName = rowValue[nameof(ProductAggregate.DisplayName)];
                    
                    if(rowValue.Keys.Any(z=>z == nameof(ProductAggregate.Description)))
                        product.Description = rowValue[nameof(ProductAggregate.Description)];

                    if (rowValue.Keys.Any(z => z == nameof(ProductAggregate.Price)))
                    {
                        var price = (string) rowValue[nameof(ProductAggregate.Price)].Replace(",", ".");
                        price = price.Replace(" ", "");
                        product.Price = string.IsNullOrWhiteSpace(price)
                            ? null
                            : decimal.Parse(price, CultureInfo.InvariantCulture);
                    }



                    if (rowValue.Keys.Any(z => z == nameof(ProductAggregate.IsEnabled)))
                    {
                          var isEnabled = rowValue[nameof(ProductAggregate.IsEnabled)];
                          product.IsEnabled = bool.Parse(isEnabled);
                    }
                    
                    if(rowValue.Keys.Any(z=>z == nameof(ProductAggregate.ShowLabel)))
                        product.ShowLabel =  bool.Parse(rowValue[nameof(ProductAggregate.ShowLabel)]);
                    
                    if(rowValue.Keys.Any(z=>z == $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Name)}"))
                        product.Supplier.Name = rowValue[$"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Name)}"];
                    
                    if(rowValue.Keys.Any(z=>z == $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.ProductLink)}"))
                        product.Supplier.ProductLink = rowValue[$"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.ProductLink)}"];

                    if (rowValue.Keys.Any(z => z == $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Link)}"))
                        product.Supplier.Link = rowValue[$"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Link)}"];
                            
                    if(rowValue.Keys.Any(z=>z == $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Logo)}"))
                        product.Supplier.Logo = rowValue[$"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Logo)}"];
                    
                    if(rowValue.Keys.Any(z=>z == $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Contact)}{nameof(Contact.Email)}"))
                        product.Supplier.Contact.Email = rowValue[$"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Contact)}{nameof(Contact.Email)}"];
                    
                    if(rowValue.Keys.Any(z=>z == $"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Contact)}{nameof(Contact.PhoneNumber)}"))
                        product.Supplier.Contact.PhoneNumber = rowValue[$"{nameof(ProductAggregate.Supplier)}{nameof(Supplier.Contact)}{nameof(Contact.PhoneNumber)}"];

                    
                    await _productsRepository.UpsertAsync(product, cancellationToken);
                }
                
            }

            return Unit.Value;
        }

      
    }
